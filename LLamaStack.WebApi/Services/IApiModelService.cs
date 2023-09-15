using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Service for handling Model requests
    /// </summary>
    public interface IApiModelService
    {

        /// <summary>
        /// Gets the model with the speified name.
        /// </summary>
        /// <param name="name">The name.</param>
        Task<ServiceResult<ModelResponse, ErrorResponse>> GetModel(string name);


        /// <summary>
        /// Gets all models.
        /// </summary>
        Task<ServiceResult<ModelsResponse, ErrorResponse>> GetModels();
    }

}
