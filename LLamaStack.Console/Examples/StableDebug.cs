using LLamaStack.StableDiffusion.Common;
using LLamaStack.StableDiffusion.Config;
using LLamaStack.StableDiffusion.Services;
using System.Diagnostics;

namespace LLamaStack.Console.Runner
{
    public class StableDebug : IExampleRunner
    {
        private readonly StableDiffusionConfig _configuration;

        public StableDebug(StableDiffusionConfig configuration)
        {
            _configuration = configuration;
        }

        public string Name => "Stable Diffusion Debug";

        public string Description => "Stable Diffusion Debugger";

        /// <summary>
        /// Stable Diffusion Demo
        /// </summary>
        public async Task RunAsync()
        {
            var prompt = "an apple wearing a hat";
            using (var stableDiffusionService = new StableDiffusionService(_configuration))
            {
                while (true)
                {
                    // Generate image using LMSDiffuser
                    await GenerateImage(stableDiffusionService, prompt, null, DiffuserType.LMSDiffuser);


                    // Generate image using EulerAncestralDiffuser
                    await GenerateImage(stableDiffusionService, prompt, null, DiffuserType.EulerAncestralDiffuser);
                }
            }
        }

        private async Task<bool> GenerateImage(IStableDiffusionService stableDiffusionService, string prompt, string negativePrompt, DiffuserType diffuserType)
        {
            var timestamp = Stopwatch.GetTimestamp();
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", $"{diffuserType}_{DateTime.Now.ToString("yyyyMMddHHmmSS")}.png");
            var diffuserConfig = new DiffuserConfig
            {
                DiffuserType = diffuserType
            };
            if (await stableDiffusionService.TextToImageFile(prompt, negativePrompt, outputPath, diffuserConfig))
            {
                OutputHelpers.WriteConsole($"{diffuserType} Image Created, FilePath: {outputPath}", ConsoleColor.Green);
                OutputHelpers.WriteConsole($"Elapsed: {Stopwatch.GetElapsedTime(timestamp)}ms", ConsoleColor.Yellow);
                return true;
            }


            return false;
        }
    }
}
