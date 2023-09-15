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


        /// <summary>
        /// Initializes a new instance of the <see cref="ApiModelService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public ApiModelService(ILogger<ApiModelService> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _models = _configuration.Models.ToImmutableDictionary(k => k.Name, FromModelConfig);
        }


        /// <summary>
        /// Gets the model with the speified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Gets all models.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Convert ModelConfig to ModelInfo
        /// </summary>
        /// <param name="modelConfig">The model configuration.</param>
        /// <returns></returns>
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
