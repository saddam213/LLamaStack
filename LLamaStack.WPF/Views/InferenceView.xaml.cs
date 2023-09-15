using LLamaStack.Core;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WPF.Commands;
using LLamaStack.WPF.Models;
using LLamaStack.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for InferenceView.xaml
    /// </summary>
    public partial class InferenceView : UserControl, INotifyPropertyChanged, ITabView
    {
        private string _prompt = string.Empty;
        private bool _isInferRunning;
        private bool _isSessionLoaded;
        private bool _isSessionLoading;
        public Guid _sessionId;
        private ModelSession<Guid> _modelSession;

        public InferenceView()
        {
            BeginSessionCommand = new AsyncRelayCommand(BeginSession, CanExecuteBeginSession);
            EndSessionCommand = new AsyncRelayCommand(EndSession, CanExecuteEndSession);
            LoadSessionCommand = new AsyncRelayCommand(LoadSession, CanExecuteLoadSession);
            SaveSessionCommand = new AsyncRelayCommand(SaveSession, CanExecuteSaveSession);
            SendPromptCommand = new AsyncRelayCommand<string>(SendPrompt, CanExecuteSendPrompt);
            CancelPromptCommand = new AsyncRelayCommand(CancelPrompt, CanExecuteCancelPrompt);
            ClearHistoryCommand = new AsyncRelayCommand(ClearHistory, CanExecuteClearHistory);
            InitializeComponent();
        }


        //TODO: Bit of an anti-pattern here, should probably bind a viewmodel with these inside
        protected ILogger<InferenceView> Logger { get; init; } = App.ServiceProvider.GetService<ILogger<InferenceView>>();
        protected IDialogService DialogService { get; init; } = App.ServiceProvider.GetService<IDialogService>();
        protected IModelSessionService<Guid> ModelSessionService { get; init; } = App.ServiceProvider.GetService<IModelSessionService<Guid>>();


        public LLamaStackConfig Configuration
        {
            get { return (LLamaStackConfig)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(LLamaStackConfig), typeof(InferenceView));

        public AsyncRelayCommand EndSessionCommand { get; }
        public AsyncRelayCommand BeginSessionCommand { get; }
        public AsyncRelayCommand LoadSessionCommand { get; }
        public AsyncRelayCommand SaveSessionCommand { get; }
        public AsyncRelayCommand<string> SendPromptCommand { get; }
        public AsyncRelayCommand CancelPromptCommand { get; }
        public AsyncRelayCommand ClearHistoryCommand { get; }

        public ObservableCollection<ModelConfiguration> Models { get; } = new ObservableCollection<ModelConfiguration>();
        public SessionConfiguration SessionConfiguration { get; set; } = new SessionConfiguration();

        private InferenceConfiguration _inferenceConfiguration = new InferenceConfiguration();
        public InferenceConfiguration InferenceConfiguration
        {
            get { return _inferenceConfiguration; }
            set { _inferenceConfiguration = value; NotifyPropertyChanged(); }
        }

        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; NotifyPropertyChanged(); }
        }

        public bool IsInferRunning
        {
            get { return _isInferRunning; }
            set { _isInferRunning = value; NotifyPropertyChanged(); }
        }

        public bool IsSessionLoaded
        {
            get { return _isSessionLoaded; }
            set { _isSessionLoaded = value; NotifyPropertyChanged(); }
        }

        public bool IsSessionLoading
        {
            get { return _isSessionLoading; }
            set { _isSessionLoading = value; NotifyPropertyChanged(); }
        }


        /// <summary>
        /// Sends the prompt.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        private async Task SendPrompt(string prompt)
        {
            Prompt = string.Empty;
            IsInferRunning = true;
            await ExecuteInference(prompt, InferenceConfiguration);
            IsInferRunning = false;
        }


        /// <summary>
        /// Determines whether this instance [can execute send prompt] the specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can execute send prompt] the specified prompt; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteSendPrompt(string prompt)
        {
            return IsSessionLoaded && prompt?.Length > 0 && !IsSessionLoading;
        }


        /// <summary>
        /// Begins the session.
        /// </summary>
        private async Task BeginSession()
        {
            IsSessionLoading = true;
            IsSessionLoaded = false;
            _modelSession = await CreateSession(SessionConfiguration, InferenceConfiguration);
            IsSessionLoaded = _modelSession is not null;
            IsSessionLoading = false;
        }

        /// <summary>
        /// Determines whether this instance [can execute begin session].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute begin session]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteBeginSession()
        {
            return SessionConfiguration?.Model is not null && !IsInferRunning && !IsSessionLoading;
        }


        /// <summary>
        /// Ends the session.
        /// </summary>
        private async Task EndSession()
        {
            await CloseSession();
            _modelSession = null;
            IsInferRunning = false;
            IsSessionLoaded = false;
        }

        /// <summary>
        /// Determines whether this instance [can execute end session].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute end session]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteEndSession()
        {
            return !IsInferRunning && !IsSessionLoading;
        }


        /// <summary>
        /// Loads the session.
        /// </summary>
        private Task LoadSession()
        {
            IsSessionLoading = true;
            IsSessionLoaded = false;
            var loadSessionView = DialogService.GetDialog<SessionLoadView>();
            if (loadSessionView.ShowDialog() == true)
            {
                var modelSession = loadSessionView.GetSeletedModel();
                if (modelSession is not null)
                {
                    _modelSession = modelSession;
                    _sessionId = modelSession.SessionId;
                    SessionConfiguration.InferenceType = modelSession.SessionConfig.InferenceType;
                    SessionConfiguration.Prompt = modelSession.SessionConfig.Prompt;
                    SessionConfiguration.AntiPrompt = modelSession.SessionConfig.AntiPrompt;
                    SessionConfiguration.OutputFilter = modelSession.SessionConfig.OutputFilter;
                    SessionConfiguration.PopulateHistory(modelSession.SessionHistory);

                    InferenceConfiguration = InferenceConfiguration.FromInferenceParams(modelSession.InferenceParams);
                }
            }
            IsSessionLoading = false;
            IsSessionLoaded = _modelSession is not null;
            return Task.CompletedTask;
        }


        /// <summary>
        /// Determines whether this instance [can execute load session].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute load session]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteLoadSession()
        {
            return !IsInferRunning && !IsSessionLoaded && !IsSessionLoading;
        }



        /// <summary>
        /// Saves the session.
        /// </summary>
        private Task SaveSession()
        {
            IsSessionLoading = true;
            var saveSessionView = DialogService.GetDialog<SessionSaveView>();
            saveSessionView.SetModelSession(_modelSession);
            saveSessionView.ShowDialog();
            IsSessionLoading = false;
            return Task.CompletedTask;
        }


        /// <summary>
        /// Determines whether this instance [can execute save session].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute save session]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteSaveSession()
        {
            return !IsInferRunning && IsSessionLoaded && !IsSessionLoading;
        }



        /// <summary>
        /// Cancels the prompt.
        /// </summary>
        private async Task CancelPrompt()
        {
            await ModelSessionService.CancelAsync(_sessionId);
            IsInferRunning = false;
        }


        /// <summary>
        /// Determines whether this instance [can execute cancel prompt].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute cancel prompt]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteCancelPrompt()
        {
            return IsInferRunning && !IsSessionLoading;
        }


        /// <summary>
        /// Clears the history.
        /// </summary>
        /// <returns></returns>
        private Task ClearHistory()
        {
            SessionConfiguration.HistoryResponses.Clear();
            return Task.CompletedTask;
        }


        /// <summary>
        /// Determines whether this instance [can execute clear history].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can execute clear history]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteClearHistory()
        {
            return SessionConfiguration?.HistoryResponses?.Count > 0 && !IsInferRunning;
        }







        #region Implemetation




        /// <summary>
        /// Creates the session.
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        private async Task<ModelSession<Guid>> CreateSession(ISessionConfig sessionConfig, IInferenceConfig inferenceConfiguration)
        {
            try
            {
                _sessionId = Guid.NewGuid();
                return await Task.Run(() => ModelSessionService.CreateAsync(_sessionId, sessionConfig, inferenceConfiguration));
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CreateSession] - {ex.Message}");
                return null;
            }
        }



        /// <summary>
        /// Closes the session.
        /// </summary>
        private async Task CloseSession()
        {
            try
            {
                await ModelSessionService.CloseAsync(_sessionId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CloseSession] - {ex.Message}");
            }
        }


        /// <summary>
        /// Executes the inference.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        private async Task ExecuteInference(string prompt, IInferenceConfig inferenceConfiguration)
        {
            try
            {
                var responseItem = CreateResponseItem(prompt);
                await Task.Run(async () =>
                {
                    await foreach (var token in ModelSessionService.InferAsync(_sessionId, prompt, inferenceConfiguration))
                    {
                        if (token.Type == InferTokenType.Begin)
                        {
                            responseItem.Timestamp = DateTime.Now;
                        }
                        else if (token.Type == InferTokenType.Content)
                        {
                            responseItem.Content += token.Content;
                        }
                        else if (token.Type == InferTokenType.End || token.Type == InferTokenType.Cancel)
                        {
                            responseItem.Signature = token.Content;
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ExecuteInference] - {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the response item.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns></returns>
        private SessionHistoryModel CreateResponseItem(string prompt)
        {
            // Prompt
            var human = new SessionHistoryModel
            {
                Content = prompt,
                Timestamp = DateTime.Now
            };

            var inference = new SessionHistoryModel
            {
                IsResponse = true
            };

            SessionConfiguration.HistoryResponses.Add(human);
            SessionConfiguration.HistoryResponses.Add(inference);
            return inference;
        }


        public void Initialize()
        {
            var lastSelected = SessionConfiguration?.Model;
            Models.Clear();
            foreach (var item in Configuration.Models)
            {
                Models.Add(ModelConfiguration.From(item));
            }

            if (!Models.Any(x => x.Name == lastSelected) && IsSessionLoaded)
            {
                EndSession().Wait();
            }

            SessionConfiguration.SelectedModel = Models.FirstOrDefault(x => x.Name == lastSelected)
                                              ?? Models.FirstOrDefault();
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}