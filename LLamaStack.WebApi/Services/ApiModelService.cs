using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Controllers;
using LLamaStack.WebApi.Models;
using Microsoft.Extensions.Options;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Service for Model related data
    /// </summary>
    /// <seealso cref="LLamaStack.WebApi.Services.IApiModelService" />
    public sealed class ApiModelService : IApiModelService
    {
        private readonly LLamaStackConfig _configuration;
        private readonly ILogger<ModelSessionController> _logger;

        public ApiModelService(ILogger<ModelSessionController> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<ServiceResult<ModelResponse, ErrorResponse>> GetModels()
        {
            _logger?.LogDebug($"GetModels");

            try
            {
                var result = new ModelResponse(_configuration.Models
                .Select(x => new ModelInfo(x.Name))
                .ToList());
                return Task.FromResult<ServiceResult<ModelResponse, ErrorResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<ServiceResult<ModelResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }
    }       
}
