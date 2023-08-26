using LLamaStack.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LLamaStack.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            // Add LLamaStack
            serviceCollection.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
            serviceCollection.AddLLamaStack<string>();

            // Add Services
            serviceCollection.AddTransient<App>();
            serviceCollection.AddTransient<AppTest>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            await serviceProvider.GetService<App>().RunAsync();
            //await serviceProvider.GetService<AppTest>().RunAsync();
        }
    }
}