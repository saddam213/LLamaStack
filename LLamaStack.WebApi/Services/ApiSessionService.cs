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


        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSessionService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="modelSessionService">The model session service.</param>
        public ApiSessionService(ILogger<ModelSessionController> logger, IModelSessionService<Guid> modelSessionService)
        {
            _logger = logger;
            _modelSessionService = modelSessionService;
        }


        /// <summary>
        /// Creates a new ModelSession.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// SessionId of the created session
        /// </returns>
        public async Task<ServiceResult<CreateResponse, ErrorResponse>> Create(CreateRequest request)
        {
            try
            {
                var sessionId = Guid.NewGuid();
                var session = await _modelSessionService.CreateAsync(sessionId, request, request);
                if (session is null)
                    return new ErrorResponse("Failed to create model session");

                _logger?.LogInformation($"Session created, SessionId: {sessionId}");
                return new CreateResponse(sessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Create] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Closes the specified ModelSession.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// true if the ModelSession was closed successfully
        /// </returns>
        public async Task<ServiceResult<CloseResponse, ErrorResponse>> Close(CloseRequest request)
        {
            try
            {
                var result = await _modelSessionService.CloseAsync(request.SessionId);

                _logger?.LogInformation($"Session closed, SessionId: {request.SessionId}");
                return new CloseResponse(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Close] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Cancels the any long running action(Infer, Save etc) the ModelSession is currently executing.
        /// </summary>
        /// <param name="request">The request.</param>
        /// true if the ModelSession action canceled successfully
        public async Task<ServiceResult<CancelResponse, ErrorResponse>> Cancel(CancelRequest request)
        {
            try
            {
                var result = await _modelSessionService.CancelAsync(request.SessionId);

                _logger?.LogInformation($"Session canceled, SessionId: {request.SessionId}");
                return new CancelResponse(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Cancel] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IAsyncEnumerable<InferTokenModel> type result for streaming of token results</returns>
        public Task<ServiceResult<InferResponse, ErrorResponse>> InferAsync(InferRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new InferResponse(_modelSessionService.InferAsync(request.SessionId, request.Prompt, request, cancellationToken));

                _logger?.LogInformation($"Session InferAsync, SessionId: {request.SessionId}");
                return Task.FromResult<ServiceResult<InferResponse, ErrorResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[InferAsync] Exception: {ex}", ex);
                return Task.FromResult<ServiceResult<InferResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }


        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IAsyncEnumerable<string> type result for streaming of token results</returns>
        public Task<ServiceResult<InferTextResponse, ErrorResponse>> InferTextAsync(InferRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new InferTextResponse(_modelSessionService.InferTextAsync(request.SessionId, request.Prompt, request, cancellationToken));

                _logger?.LogInformation($"Session InferTextAsync, SessionId: {request.SessionId}");
                return Task.FromResult<ServiceResult<InferTextResponse, ErrorResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[InferTextAsync] Exception: {ex}", ex);
                return Task.FromResult<ServiceResult<InferTextResponse, ErrorResponse>>(new ErrorResponse(ex.Message));
            }
        }


        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Completed string result of inference
        /// </returns>
        public async Task<ServiceResult<InferTextCompleteResponse, ErrorResponse>> InferTextCompleteAsync(InferRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _modelSessionService.InferTextCompleteAsync(request.SessionId, request.Prompt, request, cancellationToken);

                _logger?.LogInformation($"Session InferTextCompleteAsync, SessionId: {request.SessionId}");
                return new InferTextCompleteResponse(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[InferTextAsync] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Queue the text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Completed string result of inference
        /// </returns>
        public async Task<ServiceResult<InferTextCompleteQueuedResponse, ErrorResponse>> InferTextCompleteQueuedAsync(InferRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _modelSessionService.InferTextCompleteQueuedAsync(request.SessionId, request.Prompt, request, false, cancellationToken);

                _logger?.LogInformation($"Session InferTextCompleteQueuedAsync, SessionId: {request.SessionId}");
                return new InferTextCompleteQueuedResponse(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[InferTextAsync] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }
    }
}
