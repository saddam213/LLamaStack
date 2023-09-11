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
            builder.Logging.ClearProviders();

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



        /// <summary>
        /// LLama.cpp Native log callback.
        /// </summary>
        /// <param name="llamalevel">The llamalevel.</param>
        /// <param name="message">The message.</param>
        private static void LLamaNativeLogCallback(LLamaLogLevel llamalevel, string message)
        {
            if (string.IsNullOrEmpty(message) || message.Equals(".") || message.Equals("\n"))
                return;

            var level = llamalevel switch
            {
                LLamaLogLevel.Info => LogLevel.Information,
                LLamaLogLevel.Debug => LogLevel.Debug,
                LLamaLogLevel.Warning => LogLevel.Warning,
                LLamaLogLevel.Error => LogLevel.Error,
                _ => LogLevel.None
            };

            // Redirecting to ILogger is retardedly slow and grinds the UI to a halt on model load
            // So just call the static log function directly
            // _logger.Log(level, _logLLamaCppEvent, $"{message}".TrimEnd('\n'));
            Utils.LogToWindow($"[{DateTime.Now}] [{level}] [LLama.cpp] - {message}");
        }
    }
}
