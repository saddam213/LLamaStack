using LLamaStack.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLamaStack.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly IApiModelService _modelService;

        public ModelController(IApiModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetModels()
        {
            var response = await _modelService.GetModels();
            return response.Resolve<IActionResult>(Ok, BadRequest);
        }
    }
}