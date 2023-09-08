using LLamaStack.Core.Config;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Services
{
    public interface IModelSessionService<T> where T : IEquatable<T>, IComparable<T>
    {

        /// <summary>
        /// Gets the infer queue count.
        /// </summary>
        int InferQueueCount { get; }


        /// <summary>
        /// Gets the ModelSession with the specified Id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>The ModelSession if exists, otherwise null</returns>
        Task<ModelSession<T>> GetAsync(T sessionId);


        /// <summary>
        /// Gets all ModelSessions
        /// </summary>
        /// <returns>A collection oa all Model instances</returns>
        Task<IEnumerable<ModelSession<T>>> GetAllAsync();


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
        Task<ModelSession<T>> CreateAsync(T sessionId, ISessionConfig sessionConfig, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// Closes the session
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        Task<bool> CloseAsync(T sessionId);


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Streaming async result of <see cref="LLamaStack.Core.Models.InferTokenModel" /></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        IAsyncEnumerable<InferTokenModel> InferAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Streaming async result of <see cref="System.String" /></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        IAsyncEnumerable<string> InferTextAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default);


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
        Task<string> InferTextCompleteAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, CancellationToken cancellationToken = default);


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
        Task<string> InferTextCompleteQueuedAsync(T sessionId, string prompt, IInferenceConfig inferenceConfig = null, bool saveOnComplete = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// Cancels the current inference action.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        Task<bool> CancelAsync(T sessionId);


        /// <summary>
        /// Gets the ModelSessionState for the supplied id.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        Task<ModelSessionState<T>> GetStateAsync(T sessionId);


        /// <summary>
        /// Gets all ModelSessionStates
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ModelSessionState<T>>> GetStatesAsync();


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
        Task<ModelSession<T>> LoadStateAsync(T sessionId, CancellationToken cancellationToken = default);


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
        Task<ModelSessionState<T>> SaveStateAsync(T sessionId, CancellationToken cancellationToken = default);


        /// <summary>
        /// Removes a ModelSessionsState.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Session is already being removed</exception>
        Task<bool> RemoveStateAsync(T sessionId);
    }

}
