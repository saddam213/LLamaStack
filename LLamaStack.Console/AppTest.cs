using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core;
using LLamaStack.Core.Config;
using LLamaStack.Core.Services;

namespace LLamaStack.Console
{
    public class AppTest
    {
        private readonly LLamaStackConfig _configuration;
        private readonly IModelSessionService<int> _modelSessionService;

        public AppTest(LLamaStackConfig configuration, IModelSessionService<int> modelSessionService)
        {
            _configuration = configuration;
            _modelSessionService = modelSessionService;
        }


        public async Task RunAsync()
        {

            var prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.";

            // Create Model
            LLamaStackModel model = new LLamaStackModel(_configuration.Models.FirstOrDefault());

            // Create Context
            LLamaStackModelContext context1 = await model.CreateContext("context1");

            // Create executor
            InteractiveExecutor executor1 = new InteractiveExecutor(context1.LLamaContext);

            // Inference Parameters
            IInferenceParams inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } };


            var sessionText = "";
            OutputHelpers.WriteConsole(prompt, ConsoleColor.Yellow);
            while (true)
            {
                sessionText = OutputHelpers.ReadConsole(ConsoleColor.Green);
                if (sessionText.Equals("exit"))
                    break;

                await foreach (var text in executor1.InferAsync(sessionText, inferenceParams))
                {
                    OutputHelpers.WriteConsole(text, ConsoleColor.Cyan, false);
                }
                OutputHelpers.WriteConsole(string.Empty, ConsoleColor.Cyan);
            }
        }


    }
}