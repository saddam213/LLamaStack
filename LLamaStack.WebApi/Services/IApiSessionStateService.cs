using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Service for handling ModelSessionState requests
    /// </summary>
    public interface IApiSessionStateService
    {

        /// <summary>
        /// Gets the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>ModelSessionState object</returns>
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Get(GetRequest request);


        /// <summary>
        /// Gets all ModelSessionStates.
        /// </summary>
        /// <returns>Collection of the known ModelSessionStates</returns>
        Task<ServiceResult<List<ModelSessionState<Guid>>, ErrorResponse>> GetAll();


        /// <summary>
        /// Loads the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The loaded ModelSessionState object</returns>
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Load(LoadRequest request);


        /// <summary>
        /// Saves the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The saved ModelSessionState object</returns>
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Save(SaveRequest request);
    }
}