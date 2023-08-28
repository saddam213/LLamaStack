using LLamaStack.Core;
using LLamaStack.Core.Services;
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
            builder.Services.AddHostedService<AppService>();
            builder.Services.AddTransient<App>();
            builder.Services.AddTransient<AppTest>();

            _applicationHost = builder.Build();
            await _applicationHost.RunAsync();
        }
    }

    internal class AppService : IHostedService
    {
        private App _app;
        private AppTest _appTest;

        public AppService(App app, AppTest appTest)
        {
            _app = app;
            _appTest = appTest;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _app.RunAsync();
            // await _appTest.RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}