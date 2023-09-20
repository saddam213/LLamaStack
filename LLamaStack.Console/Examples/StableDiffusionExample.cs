using LLamaStack.StableDiffusion.Common;
using LLamaStack.StableDiffusion.Config;
using LLamaStack.StableDiffusion.Services;

namespace LLamaStack.Console.Runner
{
    public class StableDiffusionExample : IExampleRunner
    {
        private readonly StableDiffusionConfig _configuration;

        public StableDiffusionExample(StableDiffusionConfig configuration)
        {
            _configuration = configuration;
        }

        public string Name => "Stable Diffusion Demo";

        public string Description => "Creates images from the text prompt provided using all Diffuser types";

        /// <summary>
        /// Stable Diffusion Demo
        /// </summary>
        public async Task RunAsync()
        {
            while (true)
            {
                OutputHelpers.WriteConsole("Please type a prompt and press ENTER", ConsoleColor.Yellow);
                var prompt = OutputHelpers.ReadConsole(ConsoleColor.Cyan);
                using (var stableDiffusionService = new StableDiffusionService(_configuration))
                {
                    // Generate image using LMSDiffuser
                    await GenerateImage(stableDiffusionService, prompt, DiffuserType.LMSDiffuser);

                    // Generate image using EulerAncestralDiffuser
                    await GenerateImage(stableDiffusionService, prompt, DiffuserType.EulerAncestralDiffuser);
                }
            }
        }

        private async Task<bool> GenerateImage(IStableDiffusionService stableDiffusionService, string prompt, DiffuserType diffuserType)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{diffuserType}_{DateTime.Now.ToString("yyyyMMddHHmmSS")}.png");
            var diffuserConfig = new DiffuserConfig
            {
                DiffuserType = diffuserType
            };
            if (await stableDiffusionService.TextToImageFile(prompt, outputPath, diffuserConfig))
            {
                OutputHelpers.WriteConsole($"{diffuserType} Image Created, FilePath: {outputPath}", ConsoleColor.Green);
                return true;
            }
            return false;
        }
    }
}
