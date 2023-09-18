using LLamaStack.WebApi.Models;
using LLamaStack.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLamaStack.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelSessionStateController : ControllerBase
    {
        private readonly IApiSessionStateService _sessionStateService;

        public ModelSessionStateController(IApiSessionStateService sessionService)
        {
            _sessionStateService = sessionService;
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get([FromQuery] GetRequest request)
        {
            var response = await _sessionStateService.Get(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _sessionStateService.GetAll();
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Save")]
        public async Task<IActionResult> Save(SaveRequest request)
        {
            var response = await _sessionStateService.Save(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }


        [HttpPost("Load")]
        public async Task<IActionResult> Load(LoadRequest request)
        {
            var response = await _sessionStateService.Load(request);
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }

    }
}