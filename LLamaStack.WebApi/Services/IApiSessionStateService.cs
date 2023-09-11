using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    public interface IApiSessionStateService
    {
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Get(GetRequest request);
        Task<ServiceResult<List<ModelSessionState<Guid>>, ErrorResponse>> GetAll();
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Load(LoadRequest request);
        Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Save(SaveRequest request);
    }
}