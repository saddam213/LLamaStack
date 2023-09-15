using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    public class ApiSessionStateService : IApiSessionStateService
    {
        private readonly ILogger<ApiSessionStateService> _logger;
        private readonly IModelSessionService<Guid> _modelSessionService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSessionStateService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="modelSessionService">The model session service.</param>
        public ApiSessionStateService(ILogger<ApiSessionStateService> logger, IModelSessionService<Guid> modelSessionService)
        {
            _logger = logger;
            _modelSessionService = modelSessionService;
        }


        /// <summary>
        /// Gets all ModelSessionStates.
        /// </summary>
        /// <returns>
        /// Collection of the known ModelSessionStates
        /// </returns>
        public async Task<ServiceResult<List<ModelSessionState<Guid>>, ErrorResponse>> GetAll()
        {
            try
            {
                _logger?.LogInformation($"Session GetAll");

                return new List<ModelSessionState<Guid>>(await _modelSessionService.GetStatesAsync());
            }
            catch (Exception ex)
            {
                _logger?.LogError("[GetAll] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Gets the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// ModelSessionState object
        /// </returns>
        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Get(GetRequest request)
        {
            try
            {
                _logger?.LogInformation($"Session Get, SessionId: {request.SessionId}");

                var sessionState = await _modelSessionService.GetStateAsync(request.SessionId);
                if (sessionState is null)
                    return new ErrorResponse($"Session '{request.SessionId}' not found");

                return sessionState;
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Get] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Loads the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The loaded ModelSessionState object
        /// </returns>
        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Load(LoadRequest request)
        {
            try
            {
                _logger?.LogInformation($"Session Load, SessionId: {request.SessionId}");

                var sessionState = await _modelSessionService.GetStateAsync(request.SessionId);
                if (sessionState is null)
                    return new ErrorResponse($"Session '{request.SessionId}' not found");

                var modelSession = await _modelSessionService.LoadStateAsync(request.SessionId);
                if (modelSession is null)
                    return new ErrorResponse($"Failed to load model session");

                return sessionState;
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Load] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }


        /// <summary>
        /// Saves the specified ModelSessionState.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The saved ModelSessionState object
        /// </returns>
        public async Task<ServiceResult<ModelSessionState<Guid>, ErrorResponse>> Save(SaveRequest request)
        {
            try
            {
                _logger?.LogInformation($"Session Save, SessionId: {request.SessionId}");

                return await _modelSessionService.SaveStateAsync(request.SessionId);
            }
            catch (Exception ex)
            {
                _logger?.LogError("[Save] Exception: {ex}", ex);
                return new ErrorResponse(ex.Message);
            }
        }
    }
}
