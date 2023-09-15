using LLamaStack.Core.Services;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi.Services
{
    /// <summary>
    /// Service for handling ModelSession requests
    /// </summary>
    public interface IApiSessionService
    {

        /// <summary>
        /// Creates a new ModelSession.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>SessionId of the created session</returns>
        Task<ServiceResult<CreateResponse, ErrorResponse>> Create(CreateRequest request);


        /// <summary>
        /// Closes the specified ModelSession.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true if the ModelSession was closed successfully</returns>
        Task<ServiceResult<CloseResponse, ErrorResponse>> Close(CloseRequest request);


        /// <summary>
        /// Cancels the any long running action(Infer, Save etc) the ModelSession is currently executing.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<ServiceResult<CancelResponse, ErrorResponse>> Cancel(CancelRequest request);


        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IAsyncEnumerable<InferTokenModel> type result for streaming of token results</returns>
        Task<ServiceResult<InferResponse, ErrorResponse>> InferAsync(InferRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IAsyncEnumerable<string> type result for streaming of token results</returns>
        Task<ServiceResult<InferTextResponse, ErrorResponse>> InferTextAsync(InferRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Run text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed string result of inference</returns>
        Task<ServiceResult<InferTextCompleteResponse, ErrorResponse>> InferTextCompleteAsync(InferRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Queue the text inference on the ModelSession
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Completed string result of inference</returns>
        Task<ServiceResult<InferTextCompleteQueuedResponse, ErrorResponse>> InferTextCompleteQueuedAsync(InferRequest request, CancellationToken cancellationToken);
    }
}
