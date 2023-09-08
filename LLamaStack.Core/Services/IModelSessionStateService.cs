using LLamaStack.Core.Models;

namespace LLamaStack.Core.Services
{
    /// <summary>
    /// Service for handling Loading and Saving of a ModelSessions state
    /// </summary>
    /// <typeparam name="T">Type used to identify contexts</typeparam>
    public interface IModelSessionStateService<T> where T : IEquatable<T>, IComparable<T>
    {

        /// <summary>
        /// Gets the ModelSessionState with the specified identifier.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        Task<ModelSessionState<T>> GetAsync(T sessionId);


        /// <summary>
        /// Gets all ModelSessionStates.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ModelSessionState<T>>> GetAllAsync();


        /// <summary>
        /// Removes the ModelSessionState with the specified identifier.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        Task<bool> RemoveAsync(T sessionId);


        /// <summary>
        /// Loads the ModelSessionState with the specified identifier.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<ModelSessionState<T>> LoadAsync(T sessionId, CancellationToken cancellationToken = default);


        /// <summary>
        /// Saves the specified ModelSessionState.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="session">The session.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<ModelSessionState<T>> SaveAsync(T sessionId, ModelSession<T> session, CancellationToken cancellationToken = default);
    }
}
