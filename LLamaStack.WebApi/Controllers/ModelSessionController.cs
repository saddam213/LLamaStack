using LLamaStack.WebApi.Models;
using LLamaStack.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLamaStack.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelSessionController : ControllerBase
    {
        private readonly IApiSessionService _sessionService;

        public ModelSessionController(IApiSessionService sessionService)
        {
            _sessionService = sessionService;
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get(GetRequest request)
        {
            var response = await _sessionService.Get(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _sessionService.GetAll();
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var response = await _sessionService.Create(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Close")]
        public async Task<IActionResult> Close(CloseRequest request)
        {
            var response = await _sessionService.Close(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel(CancelRequest request)
        {
            var response = await _sessionService.Cancel(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Save")]
        public async Task<IActionResult> Save(SaveRequest request)
        {
            var response = await _sessionService.Save(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Load")]
        public async Task<IActionResult> Load(LoadRequest request)
        {
            var response = await _sessionService.Load(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Infer")]
        public async Task<IActionResult> Infer(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(success => Ok(success.Tokens), BadRequest);
        }


        [HttpPost("InferAsync")]
        public async Task<IActionResult> InferAsync(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(success => Ok(success.TokensAsync), BadRequest);
        }


        [HttpPost("InferText")]
        public async Task<IActionResult> InferText(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferTextAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(success => Ok(success.Tokens), BadRequest);
        }


        [HttpPost("InferTextAsync")]
        public async Task<IActionResult> InferTextAsync(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferTextAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(success => Ok(success.TokensAsync), BadRequest);
        }


        [HttpPost("InferTextCompleteAsync")]
        public async Task<IActionResult> InferTextCompleteAsync(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferTextCompleteAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("InferTextCompleteQueuedAsync")]
        public async Task<IActionResult> InferTextCompleteQueuedAsync(InferRequest request, CancellationToken cancellationToken)
        {
            var response = await _sessionService.InferTextCompleteQueuedAsync(request, cancellationToken);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }
    }
}