using LLamaStack.Core.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLamaStack.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            Options = configuration;
        }

        public LLamaStackConfig Options { get; set; }

        [BindProperty]
        public SessionConfig SessionOptions { get; set; }

        [BindProperty]
        public InferenceConfig InferenceOptions { get; set; }

        public void OnGet()
        {
            SessionOptions = new SessionConfig
            {
                Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
                AntiPrompt = "User:",
                // OutputFilter = "User:, Response:"
            };

            InferenceOptions = new InferenceConfig
            {
                Temperature = 0.8f
            };
        }
    }
}