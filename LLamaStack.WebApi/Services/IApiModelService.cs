using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    public interface IApiModelService
    {
        Task<ServiceResult<ModelResponse, ErrorResponse>> GetModels();
    }

}
