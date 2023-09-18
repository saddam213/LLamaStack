using LLama.Abstractions;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Extensions;
using LLamaStack.Core.Inference;
using LLamaStack.Core.Models;

namespace LLamaStack.Core
{
    public class ModelSession<T> where T : IEquatable<T>
    {
        private readonly T _sessionId;
        private readonly LLamaStackModel<T> _model;
        private readonly LLamaStackContext _context;
        private readonly IInferenceHandler _inferHandler;
        private readonly ISessionConfig _sessionParams;
        private readonly ITokenStreamTransform _outputTransform;
        private readonly List<SessionHistoryModel> _sessionHistory;
        private readonly IInferenceConfig _defaultInferenceConfig;

        private CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSession{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        public ModelSession(LLamaStackModel<T> model, LLamaStackContext context, T sessionId, ISessionConfig sessionConfig, IInferenceConfig inferenceParams = null)
        {
            _model = model;
            _context = context;
            _sessionId = sessionId;
            _sessionParams = sessionConfig;
            _defaultInferenceConfig = inferenceParams ?? new InferenceConfig();
            _sessionHistory = new List<SessionHistoryModel>();
            _outputTransform = CreateOutputFilter(_sessionParams);
            _inferHandler = CreateInferHandler(_model, _context, _sessionParams);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSession{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionState">State of the session.</param>
        public ModelSession(LLamaStackModel<T> model, LLamaStackContext context, ModelSessionState<T> sessionState)
             : this(model, context, sessionState.Id, sessionState.SessionConfig, sessionState.InferenceConfig)
        {
            // Load Inference state
            _inferHandler.SetStateAsync(sessionState.InferenceState);

            StateName = sessionState.Name;
            _sessionHistory = new List<SessionHistoryModel>(sessionState.SessionHistory ?? Enumerable.Empty<SessionHistoryModel>());
        }


        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        public T SessionId => _sessionId;

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        public string ModelName => _sessionParams.Model;

        /// <summary>
        /// Gets or sets the name of the session state.
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public LLamaStackContext Context => _context;

        /// <summary>
        /// Gets the session configuration.
        /// </summary>
        public ISessionConfig SessionConfig => _sessionParams;

        /// <summary>
        /// Gets the inference parameters.
        /// </summary>
        public IInferenceConfig InferenceParams => _defaultInferenceConfig;

        /// <summary>
        /// Gets the session history.
        /// </summary>
        public IEnumerable<SessionHistoryModel> SessionHistory => _sessionHistory;


        /// <summary>
        /// Create a ModelSessionState.
        /// </summary>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> CreateState(string name = null)
        {
            var inferenceState = await _inferHandler.GetStateAsync();
            return new ModelSessionState<T>
            {
                Id = _sessionId,
                Name = name ?? _sessionId.ToString(),
                Created = DateTime.UtcNow,
                ContextSize = _context.ContextSize,
                InferenceState = inferenceState,
                InferenceConfig = _defaultInferenceConfig,
                SessionConfig = _sessionParams,
                SessionHistory = _sessionHistory
            };
        }

        /// <summary>
        /// Initializes the prompt.
        /// </summary>
        /// <param name="inferenceConfig">The inference configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        internal async Task InitializePrompt(IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default)
        {
            if (_inferHandler.Type == InferenceType.Stateless)
                return;

            if (string.IsNullOrEmpty(_sessionParams.Prompt))
                return;

            // Run Initial prompt
            var inferenceParams = ConfigureInferenceParams(inferenceConfig);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await foreach (var _ in _inferHandler.InferAsync(_sessionParams.Prompt, inferenceParams, _cancellationTokenSource.Token))
            {
                // We dont really need the response of the initial prompt, so exit on first token
                break;
            };
        }


        /// <summary>
        /// Runs inference on the model context
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inferenceConfig">The inference configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        internal IAsyncEnumerable<TokenData> InferAsync(string message, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default)
        {
            var inferenceParams = ConfigureInferenceParams(inferenceConfig);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var inferenceStream = _inferHandler.InferAsync(message, inferenceParams, _cancellationTokenSource.Token);
            if (_outputTransform is not null)
                return _outputTransform.TransformAsync(inferenceStream);

            return inferenceStream;
        }


        /// <summary>
        /// Cancels the current inference.
        /// </summary>
        internal void CancelInfer()
        {
            _cancellationTokenSource?.Cancel();
        }


        /// <summary>
        /// Determines whether inference is canceled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if inference is canceled; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsInferCanceled()
        {
            return _cancellationTokenSource?.IsCancellationRequested ?? false;
        }


        /// <summary>
        /// Adds to the ModelSessionState history.
        /// </summary>
        /// <param name="sessionHistory">The session history.</param>
        /// <returns></returns>
        internal Task AddHistory(params SessionHistoryModel[] sessionHistory)
        {
            foreach (var history in sessionHistory)
            {
                _sessionHistory.Add(history);
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// Configures the inference parameters.
        /// </summary>
        /// <param name="inferenceConfig">The inference configuration.</param>
        private IInferenceParams ConfigureInferenceParams(IInferenceConfig inferenceConfig)
        {
            var inferenceParams = (inferenceConfig ?? _defaultInferenceConfig).ToInferenceParams();
            inferenceParams.AntiPrompts = _sessionParams.GetAntiPrompts();
            return inferenceParams;
        }

        private ITokenStreamTransform CreateOutputFilter(ISessionConfig sessionConfig)
        {
            var outputFilters = sessionConfig.GetOutputFilters();
            if (outputFilters.Count > 0)
                return new TokenContentKeywordTransform(outputFilters);

            return null;
        }

        private IInferenceHandler CreateInferHandler(LLamaStackModel<T> model, LLamaStackContext context, ISessionConfig sessionConfig)
        {
            return sessionConfig.InferenceType switch
            {
                InferenceType.Interactive => new InteractiveInferenceHandler<T>(_model, _context),
                InferenceType.Instruct => new InstructInferenceHandler<T>(_model, _context, sessionConfig.InputPrefix, sessionConfig.InputSuffix),
                InferenceType.Stateless => new StatelessInferenceHandler<T>(_model),
                _ => default
            };
        }

    }
}
