using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.OldVersion;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Helpers;
using LLamaStack.Core.Models;
using static LLama.StatefulExecutorBase;

namespace LLamaStack.Core
{
    public class ModelSession<T> where T : IEquatable<T>
    {
        private readonly T _sessionId;
        private readonly LLamaStackContext _context;
        private readonly ILLamaExecutor _executor;
        private readonly ISessionConfig _sessionParams;
        private readonly IPromptConfig _promptParams;
        private readonly ITextStreamTransform _outputTransform;
        private readonly List<SessionHistoryModel> _sessionHistory;

        private IInferenceParams _inferenceParams;
        private CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSession{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="sessionHistory">The session history.</param>
        public ModelSession(LLamaStackModel model, LLamaStackContext context, T sessionId, ISessionConfig sessionConfig, IInferenceParams inferenceParams = null, IEnumerable<SessionHistoryModel> sessionHistory = null)
        {
            _context = context;
            _sessionId = sessionId;
            _sessionParams = sessionConfig;
            _inferenceParams = inferenceParams;
            _sessionHistory = new List<SessionHistoryModel>(sessionHistory ?? Enumerable.Empty<SessionHistoryModel>());

            // Executor
            _executor = sessionConfig.ExecutorType switch
            {
                LLamaExecutorType.Interactive => new InteractiveExecutor(_context.LLamaContext),
                LLamaExecutorType.Instruct => new InstructExecutor(_context.LLamaContext),
                LLamaExecutorType.Stateless => new StatelessExecutor(model.LLamaWeights, model.ModelConfig),
                _ => default
            };

            // Initial Prompt
            _promptParams = new PromptConfig
            {
                Name = "Custom",
                Prompt = _sessionParams.Prompt,
                AntiPrompt = StringHelpers.CommaSeperatedToList(_sessionParams.AntiPrompt),
                OutputFilter = StringHelpers.CommaSeperatedToList(_sessionParams.OutputFilter),
            };

            //Output Filter
            if (_promptParams.OutputFilter?.Count > 0)
                _outputTransform = new LLamaTransforms.KeywordTextOutputStreamTransform(_promptParams.OutputFilter, redundancyLength: 8);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSession{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionState">State of the session.</param>
        public ModelSession(LLamaStackModel model, LLamaStackContext context, ModelSessionState<T> sessionState)
             : this(model, context, sessionState.Id, sessionState.SessionConfig, sessionState.InferenceConfig, sessionState.SessionHistory)
        {
            // Load Executor state
            if (_executor is StatefulExecutorBase statefulExecutorBase)
                statefulExecutorBase.LoadState(sessionState.ExecutorConfig);

            StateName = sessionState.Name;
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
        public IInferenceParams InferenceParams => _inferenceParams;

        /// <summary>
        /// Gets the session history.
        /// </summary>
        public IEnumerable<SessionHistoryModel> SessionHistory => _sessionHistory;


        /// <summary>
        /// Initializes the prompt.
        /// </summary>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task InitializePrompt(IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            ConfigureInferenceParams(inferenceParams);

            if (_executor is StatelessExecutor)
                return;

            // Run Initial prompt
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await foreach (var _ in _executor.InferAsync(_sessionParams.Prompt, inferenceParams, _cancellationTokenSource.Token))
            {
                // We dont really need the response of the initial prompt, so exit on first token
                break;
            };
        }


        /// <summary>
        /// Runs inference on the model context
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public IAsyncEnumerable<string> InferAsync(string message, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            ConfigureInferenceParams(inferenceParams);
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (_outputTransform is not null)
                return _outputTransform.TransformAsync(_executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token));

            return _executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token);
        }


        /// <summary>
        /// Cancels the current inference.
        /// </summary>
        public void CancelInfer()
        {
            _cancellationTokenSource?.Cancel();
        }


        /// <summary>
        /// Determines whether inference is canceled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if inference is canceled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInferCanceled()
        {
            return _cancellationTokenSource?.IsCancellationRequested ?? false;
        }


        /// <summary>
        /// Gets the ModelSessionState.
        /// </summary>
        /// <returns></returns>
        public ModelSessionState<T> GetState()
        {
            ExecutorBaseState executorState = default;
            if (_executor is StatefulExecutorBase statefulExecutorBase)
                executorState = statefulExecutorBase.GetStateData();

            return new ModelSessionState<T>
            {
                Id = _sessionId,
                Name = StateName,
                Created = DateTime.UtcNow,
                ContextSize = _context.ContextSize,
                ExecutorConfig = executorState,
                InferenceConfig = _inferenceParams,
                SessionConfig = _sessionParams,
                SessionHistory = _sessionHistory,
            };
        }


        /// <summary>
        /// Adds to the ModelSessionState history.
        /// </summary>
        /// <param name="sessionHistory">The session history.</param>
        /// <returns></returns>
        public Task AddHistory(params SessionHistoryModel[] sessionHistory)
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
        /// <param name="inferenceParams">The inference parameters.</param>
        private void ConfigureInferenceParams(IInferenceParams inferenceParams)
        {
            // If not null set as default
            if (inferenceParams is not null)
                _inferenceParams = inferenceParams;

            // If null set to new
            if (_inferenceParams is null)
                _inferenceParams = new InferenceParams();

            // Merge Antiprompts
            var antiPrompts = new List<string>();
            antiPrompts.AddRange(_promptParams.AntiPrompt ?? Enumerable.Empty<string>());
            antiPrompts.AddRange(_inferenceParams.AntiPrompts ?? Enumerable.Empty<string>());
            _inferenceParams.AntiPrompts = antiPrompts.Distinct();
        }
    }
}
