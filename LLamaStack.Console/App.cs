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
        private readonly IModelSessionService<string> _modelSessionService;

        public App(LLamaStackConfig configuration, IModelSessionService<string> modelSessionService)
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

            await _modelSessionService.CreateAsync("AppSession", sessionConfig, inferenceConfig);

            OutputHelpers.WriteConsole(sessionConfig.Prompt, ConsoleColor.Yellow);
            while (true)
            {
                var sessionText = OutputHelpers.ReadConsole(ConsoleColor.Green);
                await foreach (var token in _modelSessionService.InferAsync("AppSession", sessionText, inferenceConfig))
                {
                    if (token.Type == Core.Models.InferTokenType.Content)
                        OutputHelpers.WriteConsole(token.Content, ConsoleColor.Cyan, false);
                    else if (token.Type == Core.Models.InferTokenType.End)
                        OutputHelpers.WriteConsole(token.Content, ConsoleColor.DarkGreen, false);
                }
                OutputHelpers.WriteConsole(string.Empty, ConsoleColor.Cyan);
            }
        }
    }
}