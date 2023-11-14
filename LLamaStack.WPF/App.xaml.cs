using LLama.Common;
using LLama.Native;
using LLamaStack.Core;
using LLamaStack.WPF.Services;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LLamaStack.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _applicationHost;
        private static ILogger<App> _logger;
        private static readonly EventId _logLLamaCppEvent = new(420);


        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            var builder = Host.CreateApplicationBuilder();

            // Add LLamaStack
            builder.Services.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddWindowLogger());
            builder.Services.AddLLamaStack();

            // Add Windows
            builder.Services.AddSingleton<MainWindow>();
            builder.Services.AddTransient<SessionLoadView>();
            builder.Services.AddTransient<SessionSaveView>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            // Build App
            _applicationHost = builder.Build();
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public static IServiceProvider ServiceProvider => _applicationHost.Services;


        /// <summary>
        /// Raises the <see cref="E:Startup" /> event.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureExtendedLogging();
            await _applicationHost.StartAsync();
            ServiceProvider.GetService<MainWindow>().Show();
        }


        /// <summary>
        /// Raises the <see cref="E:Exit" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ExitEventArgs"/> instance containing the event data.</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            await _applicationHost.StopAsync();
            base.OnExit(e);
        }


        /// <summary>
        /// Configures the extened logging.
        /// </summary>
        private void ConfigureExtendedLogging()
        {
            _logger = ServiceProvider.GetService<ILogger<App>>();

            // Setup Extra Logging
            NativeApi.llama_log_set(LLamaNativeLogCallback);

            // Try catch any deeper exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) => _logger.LogError($"UnhandledException: {e.ExceptionObject}");
            Current.DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            TaskScheduler.UnobservedTaskException += (s, e) => _logger.LogError($"UnobservedTaskException: {e.Exception?.Message}");
        }

        private static void LLamaNativeLogCallback(LLamaLogLevel llamaLogLevel, string message)
        {
            var level = llamaLogLevel switch
            {
                LLamaLogLevel.Warning => LogLevel.Warning,
                LLamaLogLevel.Info => LogLevel.Information,
                LLamaLogLevel.Error => LogLevel.Error,
                LLamaLogLevel.Debug => LogLevel.Debug,
                _ => LogLevel.Debug,
            };

            var filtered = message.TrimEnd(new[] { '\n', '.' });
            if (string.IsNullOrEmpty(filtered) || filtered.StartsWith("llama_model_loader:")) // To much for the poor window logger
                return;

            _logger.Log(level, filtered);
        }
    }
}
