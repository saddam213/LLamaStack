using LLamaStack.Core;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Services;

/// <summary>
/// 
/// </summary>
namespace LLamaStack.Console
{
    public class App
    {
        private readonly LLamaStackConfig _configuration;
        private readonly IModelSessionService<int> _modelSessionService;

        public App(LLamaStackConfig configuration, IModelSessionService<int> modelSessionService)
        {
            _configuration = configuration;
            _modelSessionService = modelSessionService;
        }

        public async Task RunAsync()
        {
            var sessionConfig = new SessionConfig
            {
                Model = "WizardLM-7B",
                Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
                ExecutorType = ExecutorType.Instruct
            };

            var inferenceConfig = new InferenceConfig
            {
                Temperature = 0.8f
            };

            await _modelSessionService.CreateAsync(1, sessionConfig, inferenceConfig);

            await Task.WhenAll(
                HandleResult("What is an apple?", inferenceConfig),
                HandleResult("What is an lemon?", inferenceConfig),
                HandleResult("What is an banana?", inferenceConfig),
                HandleResult("What is an terrorist?", inferenceConfig), 
                HandleResult("What is an jew?", inferenceConfig));
        }

        private async Task HandleResult(string prompt, InferenceConfig inferenceConfig)
        {
            OutputHelpers.WriteConsole(prompt, ConsoleColor.Green);
            var result = await _modelSessionService.QueueInferTextAsync(1, prompt, inferenceConfig);
            OutputHelpers.WriteConsole(prompt, ConsoleColor.Yellow);
            OutputHelpers.WriteConsole(result, ConsoleColor.Cyan);
            OutputHelpers.WriteConsole("-----------------------------------------", ConsoleColor.Red);
        }
    }
}