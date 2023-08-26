using LLamaStack.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LLamaStack.Console
{
    internal class Program
    {
        private static IHost _applicationHost;

        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();

            // Add LLamaStack
            builder.Services.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace));
            builder.Services.AddLLamaStack<string>();

            // Add Services
            builder.Services.AddTransient<App>();
            builder.Services.AddTransient<AppTest>();

            _applicationHost = builder.Build();
            await _applicationHost.RunAsync();

            await ServiceProvider.GetService<App>().RunAsync();
            //await ServiceProvider.GetService<AppTest>().RunAsync();
        }

        public static IServiceProvider ServiceProvider => _applicationHost.Services;
    }
}