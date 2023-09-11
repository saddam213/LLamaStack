using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;
using System.Collections.Immutable;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Service for Model related data
    /// </summary>
    /// <seealso cref="LLamaStack.WebApi.Services.IApiModelService" />
    public sealed class ApiModelService : IApiModelService
    {
        private readonly LLamaStackConfig _configuration;
        private readonly ILogger<ApiModelService> _logger;
        private readonly ImmutableDictionary<string, ModelInfo> _models;

        public ApiModelService(ILogger<ApiModelService> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _models = _configuration.Models.ToImmutableDictionary(k => k.Name, FromModelConfig);
        }

        public Task<ServiceResult<ModelResponse, ErrorResponse>> GetModel(string name)
        {
            try
            {
                ServiceResult<ModelResponse, ErrorResponse> response = _models.TryGetValue(name, out var modelInfo)
                    ? new ModelResponse(modelInfo)
                    : new ErrorResponse($"Model '{name}' not found");

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<ServiceResult<ModelResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }

        public Task<ServiceResult<ModelsResponse, ErrorResponse>> GetModels()
        {
            try
            {
                return Task.FromResult<ServiceResult<ModelsResponse, ErrorResponse>>(new ModelsResponse(_models.Values));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<ServiceResult<ModelsResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }

        private static ModelInfo FromModelConfig(ModelConfig modelConfig)
        {
            return new ModelInfo(modelConfig.Name)
            {
                BatchSize = modelConfig.BatchSize,
                ContextSize = modelConfig.ContextSize,
                Encoding = modelConfig.Encoding,
                MaxInstances = modelConfig.MaxInstances
            };
        }
    }
}
