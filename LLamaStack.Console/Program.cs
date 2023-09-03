using LLamaStack.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace LLamaStack.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Logging.ClearProviders();

            // Add LLamaStack
            builder.Services.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Error));
            builder.Services.AddLLamaStack<string>();

            // Add AppService
            builder.Services.AddHostedService<AppService>();

            // Add Runners
            var exampleRunners = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(IExampleRunner).IsAssignableFrom(type) && !type.IsInterface)
                .ToList();
            builder.Services.AddSingleton(exampleRunners.AsEnumerable());
            foreach (var exampleRunner in exampleRunners)
            {
                builder.Services.AddSingleton(exampleRunner);
            }

            // Start
            await builder.Build().RunAsync();
        }

    }
}