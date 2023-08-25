using LLama.Abstractions;
using LLamaStack.Core;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WPF.Commands;
using LLamaStack.WPF.Services;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;


namespace LLamaStack.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly LLamaStackConfig _configuration;
        private readonly IDialogService _dialogService;
        private readonly IModelSessionService<Guid> _modelSessionService;
        private string _prompt;
        private string _outputLog;
        private bool _isInferRunning;
        private bool _isSessionLoaded;
        private bool _isSessionLoading;
        public Guid _sessionId;
        private ModelSession<Guid> _modelSession;


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="modelSessionService">The model session service.</param>
        public MainWindow(ILogger<MainWindow> logger, LLamaStackConfig configuration, IModelSessionService<Guid> modelSessionService, IDialogService dialogService)
        {
            _logger = logger;
            _configuration = configuration;
            _dialogService = dialogService;
            _modelSessionService = modelSessionService;
            Models = new ObservableCollection<ModelConfig>(_configuration.Models);
            BeginSessionCommand = new AsyncRelayCommand(BeginSession, CanExecuteBeginSession);
            EndSessionCommand = new AsyncRelayCommand(EndSession, CanExecuteEndSession);
            LoadSessionCommand = new AsyncRelayCommand(LoadSession, CanExecuteLoadSession);
            SaveSessionCommand = new AsyncRelayCommand(SaveSession, CanExecuteSaveSession);
            SendPromptCommand = new AsyncRelayCommand<string>(SendPrompt, CanExecuteSendPrompt);
            CancelPromptCommand = new AsyncRelayCommand(CancelPrompt, CanExecuteCancelPrompt);
            ClearHistoryCommand = new AsyncRelayCommand(ClearHistory, CanExecuteClearHistory);
            LoadDefaultValues();
            InitializeComponent();
        }


        public AsyncRelayCommand EndSessionCommand { get; }
        public AsyncRelayCommand BeginSessionCommand { get; }
        public AsyncRelayCommand LoadSessionCommand { get; }
        public AsyncRelayCommand SaveSessionCommand { get; }
        public AsyncRelayCommand<string> SendPromptCommand { get; }
        public AsyncRelayCommand CancelPromptCommand { get; }
        public AsyncRelayCommand ClearHistoryCommand { get; }

        public ObservableCollection<ModelConfig> Models { get; }
        public SessionConfiguration SessionConfiguration { get; set; }

        private InferenceConfiguration _inferenceConfiguration;
        public InferenceConfiguration InferenceConfiguration
        {
            get { return _inferenceConfiguration; }
            set { _inferenceConfiguration = value; NotifyPropertyChanged(); }
        }

        #region UI Members

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

        public string OutputLog
        {
            get { return _outputLog; }
            set { _outputLog = value; NotifyPropertyChanged(); }
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
            var loadSessionView = _dialogService.GetDialog<SessionLoadView>();
            if (loadSessionView.ShowDialog() == true)
            {
                var modelSession = loadSessionView.GetSeletedModel();
                if (modelSession is not null)
                {
                    _modelSession = modelSession;
                    _sessionId = modelSession.SessionId;
                    SessionConfiguration.ExecutorType = modelSession.SessionConfig.ExecutorType;
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
            var saveSessionView = _dialogService.GetDialog<SessionSaveView>();
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
            await _modelSessionService.CancelAsync(_sessionId);
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
            return SessionConfiguration.HistoryResponses.Count > 0 && !IsInferRunning;
        }


        /// <summary>
        /// Updates the output log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void UpdateOutputLog(string message)
        {
            OutputLog += message;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion



        #region Implemetation

        /// <summary>
        /// Loads the default values.
        /// </summary>
        private void LoadDefaultValues()
        {
            // Prompt = "what is a tree?";
            SessionConfiguration = new SessionConfiguration
            {
                SelectedModel = _configuration.Models.FirstOrDefault(),
                ExecutorType = ExecutorType.Instruct
            };
            InferenceConfiguration = new InferenceConfiguration();
        }


        /// <summary>
        /// Creates the session.
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        private async Task<ModelSession<Guid>> CreateSession(ISessionConfig sessionConfig, IInferenceParams inferenceParams)
        {
            try
            {
                _sessionId = Guid.NewGuid();
                return await Task.Run(() => _modelSessionService.CreateAsync(_sessionId, sessionConfig, inferenceParams));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CreateSession] - {ex.Message}");
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
                await _modelSessionService.CloseAsync(_sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CloseSession] - {ex.Message}");
            }
        }


        /// <summary>
        /// Executes the inference.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        private async Task ExecuteInference(string prompt, IInferenceParams inferenceParams)
        {
            try
            {
                var responseItem = CreateResponseItem(prompt);
                await Task.Run(async () =>
                {
                    await foreach (var token in _modelSessionService.InferAsync(_sessionId, prompt, inferenceParams))
                    {
                        if (token.Type == InferTokenType.Begin)
                        {
                            responseItem.Timestamp = DateTime.Now;
                            continue;
                        }

                        if (token.Type == InferTokenType.End)
                        {
                            responseItem.Signature = token.Content;
                            break;
                        }
                        responseItem.Content += token.Content;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExecuteInference] - {ex.Message}");
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

        #endregion
    }
}