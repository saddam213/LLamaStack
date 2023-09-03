using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Controllers;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Simple service to wrappper for the ModelSessionService to handle errors, extra functionality etc
    /// </summary>
    /// <seealso cref="LLamaStack.WebApi.Services.IApiSessionService" />
    public sealed class ApiSessionService : IApiSessionService
    {
        private readonly ILogger<ModelSessionController> _logger;
        private readonly IModelSessionService<Guid> _modelSessionService;

        public ApiSessionService(ILogger<ModelSessionController> logger, IModelSessionService<Guid> modelSessionService)
        {
            _logger = logger;
            _modelSessionService = modelSessionService;
        }


        public async Task<ServiceResult<CreateResponse, ErrorResponse>> Create(CreateRequest request)
        {
            _logger?.LogDebug($"Create");

            try
            {
                var sessionId = Guid.NewGuid();
                var session = await _modelSessionService.CreateAsync(sessionId, request, request.ToInferenceParams());
                if (session is null)
                    return new ErrorResponse("Failed to create model session");

                _logger?.LogDebug($"Session created, SessionId: {sessionId}");
                return new CreateResponse(sessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResult<CloseResponse, ErrorResponse>> Close(CloseRequest request)
        {
            _logger?.LogDebug($"Close");

            try
            {
                return new CloseResponse(await _modelSessionService.CloseAsync(request.SessionId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResult<CancelResponse, ErrorResponse>> Cancel(CancelRequest request)
        {
            _logger?.LogDebug($"Cancel");

            try
            {
                return new CancelResponse(await _modelSessionService.CancelAsync(request.SessionId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }


        public Task<ServiceResult<InferResponse, ErrorResponse>> InferAsync(InferRequest request, CancellationToken cancellationToken)
        {
            _logger?.LogDebug($"InferAsync, SessionId: {request.SessionId}");

            try
            {
                var response = new InferResponse(_modelSessionService.InferAsync(request.SessionId, request.Prompt, request.ToInferenceParams(), cancellationToken));
                return Task.FromResult<ServiceResult<InferResponse, ErrorResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<ServiceResult<InferResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }


        public Task<ServiceResult<InferTextResponse, ErrorResponse>> InferTextAsync(InferRequest request, CancellationToken cancellationToken)
        {
            _logger?.LogDebug($"InferTextAsync, SessionId: {request.SessionId}");

            async IAsyncEnumerable<string> ExecuteInferAsync()
            {
                await foreach (var token in _modelSessionService.InferAsync(request.SessionId, request.Prompt, request.ToInferenceParams(), cancellationToken).ConfigureAwait(false))
                {
                    if (token.Type == InferTokenType.Content)
                        yield return token.Content;
                }
            }

            try
            {
                var response = new InferTextResponse(ExecuteInferAsync());
                return Task.FromResult<ServiceResult<InferTextResponse, ErrorResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<ServiceResult<InferTextResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }

        public async Task<ServiceResult<List<ModelSessionState<Guid>>, ErrorResponse>> GetAll()
        {
            try
            {
                return new List<ModelSessionState<Guid>>(await _modelSessionService.GetStatesAsync());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Get(GetRequest request)
        {
            try
            {
                return await _modelSessionService.GetStateAsync(request.SessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Load(LoadRequest request)
        {
            try
            {
                var modelSession = await _modelSessionService.LoadStateAsync(request.SessionId);
                return await _modelSessionService.GetStateAsync(request.SessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Save(SaveRequest request)
        {
            try
            {
                return await _modelSessionService.SaveStateAsync(request.SessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return new ErrorResponse(ex.Message);
            }
        }
    }
}
