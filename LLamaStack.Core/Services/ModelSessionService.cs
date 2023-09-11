using LLamaStack.Core.Async;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaStack.Core.Services
{
    /// <summary>
    /// Service for handling model sessions
    /// </summary>
    /// <typeparam name="T">Type used for the session identifier</typeparam>
    /// <seealso cref="LLamaStack.Core.Services.IModelSessionService&lt;T&gt;" />
    public class ModelSessionService<T> : IModelSessionService<T> where T : IEquatable<T>, IComparable<T>
    {
        private readonly AsyncGuard<T> _sessionGuard;
        private readonly IModelService<T> _modelService;
        private readonly AsyncQueue<InferQueueItem, string> _inferTextQueue;
        private readonly IModelSessionStateService<T> _modelSessionStateService;
        private readonly ConcurrentDictionary<T, ModelSession<T>> _modelSessions;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSessionService{T}"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        /// <param name="modelSessionStateService">The model session state service.</param>
        public ModelSessionService(IModelService<T> modelService, IModelSessionStateService<T> modelSessionStateService)
        {
            _modelService = modelService;
            _modelSessionStateService = modelSessionStateService;
            _sessionGuard = new AsyncGuard<T>();
            _modelSessions = new ConcurrentDictionary<T, ModelSession<T>>();
            _inferTextQueue = new AsyncQueue<InferQueueItem, string>(ProcessInferQueueAsync);
        }


        /// <summary>
        /// Gets the infer queue count.
        /// </summary>
        public int InferQueueCount => _inferTextQueue.QueueCount();


        /// <summary>
        /// Gets the ModelSession with the specified Id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The ModelSession if exists, otherwise null</returns>
        public Task<ModelSession<T>> GetAsync(T sessionId)
        {
            return Task.FromResult(_modelSessions.TryGetValue(sessionId, out var session) ? session : null);
        }


        /// <summary>
        /// Gets all ModelSessions
        /// </summary>
        /// <returns>A collection oa all Model instances</returns>
        public Task<IEnumerable<ModelSession<T>>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ModelSession<T>>>(_modelSessions.Values);
        }


        /// <summary>
        /// Creates a new ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceConfig">The default inference configuration, will be used for all inference where no infer configuration is supplied.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session with id {sessionId} already exists
        /// or
        /// Failed to create model session
        /// </exception>
        public async Task<ModelSession<T>> CreateAsync(T sessionId, ISessionConfig sessionConfig, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default)
        {
            if (_modelSessions.TryGetValue(sessionId, out _))
                throw new Exception($"Session with id {sessionId} already exists");

            // Create context
            var (model, context) = await _modelService.GetOrCreateModelAndContext(sessionConfig.Model, sessionId);

            // Create session
            var modelSession = new ModelSession<T>(model, context, sessionId, sessionConfig, inferenceConfig);
            if (!_modelSessions.TryAdd(sessionId, modelSession))
                throw new Exception($"Failed to create model session");

            // Run initial Prompt
            await modelSession.InitializePrompt(inferenceConfig, cancellationToken);
            return modelSession;

        }


        /// <summary>
        /// Closes the session
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<bool> CloseAsync(T sessionId)
        {
            if (_modelSessions.TryRemove(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return await _modelService.RemoveContext(modelSession.ModelName, sessionId);
            }
            return false;
        }


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Streaming async result of <see cref="LLamaStack.Core.Models.InferTokenModel" /></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public async IAsyncEnumerable<InferTokenModel> InferAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!_sessionGuard.Guard(sessionId))
                throw new Exception($"Inference is already running for this session");

            try
            {
                if (!_modelSessions.TryGetValue(sessionId, out var modelSession))
                    yield break;


                // Send begin of response
                var response = new StringBuilder();
                var stopwatch = Stopwatch.GetTimestamp();
                var promptHistory = new SessionHistoryModel(prompt);
                yield return new InferTokenModel(default, default, default, InferTokenType.Begin, GetElapsed(stopwatch));

                // Send content of response
                await foreach (var token in modelSession.InferAsync(prompt, inferenceConfig, cancellationToken).ConfigureAwait(false))
                {
                    response.Append(token);
                    yield return new InferTokenModel(default, default, token, InferTokenType.Content, GetElapsed(stopwatch));

                    // TODO:Revisit: Help ensure that the IAsyncEnumerable is properly scheduled for asynchronous execution as nothing in the upstream loop is awaited
                    await Task.Yield();
                }

                // Send end of response
                var elapsedTime = GetElapsed(stopwatch);
                var endTokenType = modelSession.IsInferCanceled() ? InferTokenType.Cancel : InferTokenType.End;
                var signature = endTokenType == InferTokenType.Cancel
                      ? $"Inference cancelled after {elapsedTime / 1000:F0} seconds"
                      : $"Inference completed in {elapsedTime / 1000:F0} seconds";
                yield return new InferTokenModel(default, default, signature, endTokenType, elapsedTime);

                // Add History
                await modelSession.AddHistory(promptHistory, new SessionHistoryModel(response.ToString(), signature));
            }
            finally
            {
                _sessionGuard.Release(sessionId);
            }
        }


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Streaming async result of <see cref="System.String" /></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public IAsyncEnumerable<string> InferTextAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default)
        {
            async IAsyncEnumerable<string> InferTextInternal()
            {
                await foreach (var token in InferAsync(sessionId, prompt, inferenceConfig, cancellationToken).ConfigureAwait(false))
                {
                    if (token.Type == InferTokenType.Content)
                        yield return token.Content;
                }
            }
            return InferTextInternal();
        }


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed inference result as string</returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public async Task<string> InferTextCompleteAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default)
        {
            var inferResult = await InferAsync(sessionId, prompt, inferenceConfig, cancellationToken)
                .Where(x => x.Type == InferTokenType.Content)
                .Select(x => x.Content)
                .ToListAsync(cancellationToken: cancellationToken);

            return string.Concat(inferResult);
        }



        /// <summary>
        /// Queues inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="saveOnComplete">Save the session after the queue processes the request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed inference result as string</returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public async Task<string> InferTextCompleteQueuedAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, bool saveOnComplete = false, CancellationToken cancellationToken = default)
        {
            return await _inferTextQueue.QueueItem(new InferQueueItem(sessionId, prompt, inferenceConfig, saveOnComplete, cancellationToken));
        }


        /// <summary>
        /// Cancels the current inference action.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public Task<bool> CancelAsync(T sessionId)
        {
            if (_modelSessions.TryGetValue(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }


        /// <summary>
        /// Gets the ModelSessionState for the supplied id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> GetStateAsync(T sessionId)
        {
            return await _modelSessionStateService.GetAsync(sessionId);
        }


        /// <summary>
        /// Gets all ModelSessionStates
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ModelSessionState<T>>> GetStatesAsync()
        {
            return await _modelSessionStateService.GetAllAsync();
        }


        /// <summary>
        /// Loads a ModelSession from a saved ModelSessionState.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session is already loading
        /// or
        /// Failed to load model session state
        /// or
        /// Failed to add model session state
        /// </exception>
        public async Task<ModelSession<T>> LoadStateAsync(T sessionId, CancellationToken cancellationToken = default)
        {
            if (!_sessionGuard.Guard(sessionId))
                throw new Exception($"Session is already loading");

            try
            {
                // Close and remove existing session
                await CloseAsync(sessionId);

                // Load session state
                var modelSessionState = await _modelSessionStateService.LoadAsync(sessionId);
                if (modelSessionState is null)
                    throw new Exception($"Failed to load model session state");

                // Create context
                var (model, context) = await _modelService.GetOrCreateModelAndContext(modelSessionState.SessionConfig.Model, sessionId);

                // Load context state
                await context.LoadStateAsync(modelSessionState.ContextFile);

                // Create session
                var modelSession = new ModelSession<T>(model, context, modelSessionState);

                if (!_modelSessions.TryAdd(sessionId, modelSession))
                    throw new Exception($"Failed to add model session state");

                return modelSession;
            }
            finally
            {
                _sessionGuard.Release(sessionId);
            }
        }


        /// <summary>
        /// Saves a ModelSession to a ModelSessionState
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session is already loading
        /// or
        /// Failed to save model session state
        /// </exception>
        public async Task<ModelSessionState<T>> SaveStateAsync(T sessionId, CancellationToken cancellationToken = default)
        {
            if (!_sessionGuard.Guard(sessionId))
                throw new Exception($"Session is already loading");

            try
            {
                if (!_modelSessions.TryGetValue(sessionId, out var modelSession))
                    return null;

                var modelSessionState = await _modelSessionStateService.SaveAsync(sessionId, modelSession);
                if (modelSessionState is null)
                    throw new Exception($"Failed to save model session state");

                //Save context state
                await modelSession.Context.SaveStateAsync(modelSessionState.ContextFile);

                return modelSessionState;
            }
            finally
            {
                _sessionGuard.Release(sessionId);
            }
        }


        /// <summary>
        /// Removes a ModelSessionsState.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Session is already being removed</exception>
        public async Task<bool> RemoveStateAsync(T sessionId)
        {
            if (!_sessionGuard.Guard(sessionId))
                throw new Exception($"Session is already being removed");

            try
            {
                await CloseAsync(sessionId);
                return await _modelSessionStateService.RemoveAsync(sessionId);
            }
            finally
            {
                _sessionGuard.Release(sessionId);
            }
        }


        /// <summary>
        /// Gets the elapsed time in milliseconds.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        private static int GetElapsed(long timestamp)
        {
            return (int)Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
        }


        /// <summary>
        /// Processes the infer queue.
        /// </summary>
        /// <param name="inferQueueItem">The infer queue item.</param>
        /// <returns></returns>
        private async Task<string> ProcessInferQueueAsync(InferQueueItem inferQueueItem)
        {
            var inferenceResult = await InferTextCompleteAsync(inferQueueItem.SessionId, inferQueueItem.Prompt, inferQueueItem.InferenceConfig, inferQueueItem.CancellationToken);
            if (inferQueueItem.SaveOnComplete)
                await SaveStateAsync(inferQueueItem.SessionId, inferQueueItem.CancellationToken);

            return inferenceResult;
        }


        /// <summary>
        /// Record for the Infer queue
        /// </summary>
        private record InferQueueItem(T SessionId, string Prompt, IInferenceConfig InferenceConfig, bool SaveOnComplete, CancellationToken CancellationToken = default);
    }
}
