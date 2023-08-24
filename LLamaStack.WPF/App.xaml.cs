using LLama.Common;
using LLamaStack.Core;
using LLamaStack.WPF.Services;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
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
        private static ILogger<App> _logger;
        private readonly ServiceProvider _serviceProvider;
        private static readonly EventId _logLLamaCppEvent = new(420);

        public App()
        {
            var serviceCollection = new ServiceCollection();

            // Add LLamaStack
            serviceCollection.AddLogging((loggingBuilder) => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddWindowLogger());
            serviceCollection.AddLLamaStack();

            // Add Windows
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddTransient<SessionLoadView>();
            serviceCollection.AddTransient<SessionSaveView>();
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<App>>();

            // Setup Extra Logging
            LLama.Native.NativeApi.llama_log_set(LLamaNativeLogCallback);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => _logger.LogError($"UnhandledException: {e.ExceptionObject}");
            Current.DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            DispatcherUnhandledException += (s, e) => _logger.LogError($"DispatcherUnhandledException: {e.Exception?.Message}");
            TaskScheduler.UnobservedTaskException += (s, e) => _logger.LogError($"UnobservedTaskException: {e.Exception?.Message}");
        }

        public ServiceProvider ServiceProvider => _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _serviceProvider.GetService<MainWindow>().Show();
        }

        private static void LLamaNativeLogCallback(ILLamaLogger.LogLevel llamalevel, string message)
        {
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
