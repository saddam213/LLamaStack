using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Services;

namespace LLamaStack.Console.Runner
{
    public class BasicInferExample : IExampleRunner
    {
        private readonly IModelSessionService<string> _modelSessionService;

        public BasicInferExample(IModelSessionService<string> modelSessionService)
        {
            _modelSessionService = modelSessionService;
        }

        public string Name => "Basic Inference Demo";

        public string Description => "Creates a Instruct session using ModelSessionService and waits for input";

        /// <summary>
        /// Basic inference example, create a session, start a question loop
        /// </summary>
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