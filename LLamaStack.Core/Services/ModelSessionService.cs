using LLama.Abstractions;
using LLamaStack.Core.Config;
using LLamaStack.Core.Helpers;
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
    public class ModelSessionService<T> : IModelSessionService<T> where T : IEquatable<T>
    {
        private readonly AsyncGuard<T> _sessionGuard;
        private readonly IModelService _modelService;
        private readonly IModelSessionStateService<T> _modelSessionStateService;
        private readonly ConcurrentDictionary<T, ModelSession<T>> _modelSessions;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSessionService{T}"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        /// <param name="modelSessionStateService">The model session state service.</param>
        public ModelSessionService(IModelService modelService, IModelSessionStateService<T> modelSessionStateService)
        {
            _modelService = modelService;
            _modelSessionStateService = modelSessionStateService;
            _sessionGuard = new AsyncGuard<T>();
            _modelSessions = new ConcurrentDictionary<T, ModelSession<T>>();
        }


        /// <summary>
        /// Gets the ModelSessionState for the supplied id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<ModelSessionState<T>> GetAsync(T sessionId)
        {
            return await _modelSessionStateService.GetAsync(sessionId);
        }


        /// <summary>
        /// Gets the current ModelSessionStates
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ModelSessionState<T>>> GetAllAsync()
        {
            return await _modelSessionStateService.GetAllAsync();
        }


        /// <summary>
        /// Creates a new ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session with id {sessionId} already exists
        /// or
        /// Failed to create model session
        /// </exception>
        public async Task<ModelSession<T>> CreateAsync(T sessionId, ISessionConfig sessionConfig, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            if (_modelSessions.TryGetValue(sessionId, out _))
                throw new Exception($"Session with id {sessionId} already exists");

            // Create context
            var context = await _modelService.GetOrCreateModelAndContext(sessionConfig.Model, sessionId.ToString());

            // Create session
            var modelSession = new ModelSession<T>(context, sessionId, sessionConfig, inferenceParams);
            if (!_modelSessions.TryAdd(sessionId, modelSession))
                throw new Exception($"Failed to create model session");

            // Run initial Prompt
            await modelSession.InitializePrompt(inferenceParams, cancellationToken);
            return modelSession;

        }


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public async IAsyncEnumerable<InferTokenModel> InferAsync(T sessionId, string prompt, IInferenceParams inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                await foreach (var token in modelSession.InferAsync(prompt, inferenceParams, cancellationToken))
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
        /// Closes the session
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<bool> CloseAsync(T sessionId)
        {
            if (_modelSessions.TryRemove(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return await _modelService.RemoveContext(modelSession.ModelName, sessionId.ToString());
            }
            return false;
        }


        /// <summary>
        /// Cancels the current action.
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
        public async Task<ModelSession<T>> LoadAsync(T sessionId, CancellationToken cancellationToken = default)
        {
            if (!_sessionGuard.Guard(sessionId))
                throw new Exception($"Session is already loading");

            try
            {
                var modelSessionState = await _modelSessionStateService.LoadAsync(sessionId);
                if (modelSessionState is null)
                    throw new Exception($"Failed to load model session state");

                // Create context state
                var context = await _modelService.GetOrCreateModelAndContext(modelSessionState.SessionConfig.Model, sessionId.ToString());

                // Load context state
                await context.LoadStateAsync(modelSessionState.ContextFile);

                // Create session
                var modelSession = new ModelSession<T>(context, modelSessionState);

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
        public async Task<ModelSessionState<T>> SaveAsync(T sessionId, CancellationToken cancellationToken = default)
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
        public async Task<bool> RemoveAsync(T sessionId)
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
    }
}
