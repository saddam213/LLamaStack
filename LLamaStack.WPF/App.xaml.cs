using LLama.Common;
using LLamaStack.Core;
using LLamaStack.WPF.Services;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;

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
            LLama.Native.NativeApi.llama_log_set(LLamaNativeLogCallback);

            // Try catch any deeper exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) => _logger.LogError($"UnhandledException: {e.ExceptionObject}");
            Current.DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            TaskScheduler.UnobservedTaskException += (s, e) => _logger.LogError($"UnobservedTaskException: {e.Exception?.Message}");
        }


        /// <summary>
        /// LLama.cpp Native log callback.
        /// </summary>
        /// <param name="llamalevel">The llamalevel.</param>
        /// <param name="message">The message.</param>
        private static void LLamaNativeLogCallback(ILLamaLogger.LogLevel llamalevel, string message)
        {
            if (string.IsNullOrEmpty(message) || message.Equals(".") || message.Equals("\n"))
                return;

            var level = llamalevel switch
            {
                ILLamaLogger.LogLevel.Info => LogLevel.Information,
                ILLamaLogger.LogLevel.Debug => LogLevel.Debug,
                ILLamaLogger.LogLevel.Warning => LogLevel.Warning,
                ILLamaLogger.LogLevel.Error => LogLevel.Error,
                _ => LogLevel.None
            };
            _logger.Log(level, _logLLamaCppEvent, $"{message}".TrimEnd('\n'));
        }
    }
}
