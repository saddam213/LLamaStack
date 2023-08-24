using LLamaStack.Core;
using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using LLamaStack.WPF.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for SessionSaveView.xaml
    /// </summary>
    public partial class SessionSaveView : Window, INotifyPropertyChanged
    {
        private bool _isSaving = false;
        private string _sessionName = "New Session";
        private ModelSession<Guid> _modelSession;
        private readonly LLamaStackConfig _configuration;
        private readonly ILogger<SessionLoadView> _logger;
        private readonly IModelSessionService<Guid> _modelSessionService;

        public SessionSaveView(ILogger<SessionLoadView> logger, LLamaStackConfig configuration, IModelSessionService<Guid> modelSessionService)
        {
            _logger = logger;
            _configuration = configuration;
            _modelSessionService = modelSessionService;
            SaveCommand = new AsyncRelayCommand(Save, CanExecuteSave);
            CancelCommand = new AsyncRelayCommand(Cancel, CanExecuteCancel);
            InitializeComponent();
        }

        public AsyncRelayCommand SaveCommand { get; }
        public AsyncRelayCommand CancelCommand { get; }

        public string SessionName
        {
            get { return _sessionName; }
            set { _sessionName = value; NotifyPropertyChanged(); }
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set { _isSaving = value; NotifyPropertyChanged(); }
        }


        public void SetModelSession(ModelSession<Guid> modelSession)
        {
            _modelSession = modelSession;
        }

        private Task Cancel()
        {
            DialogResult = false;
            return Task.CompletedTask;
        }


        private bool CanExecuteCancel()
        {
            return !IsSaving;
        }



        private async Task Save()
        {
            IsSaving = true;
            await SaveModelSession();
            IsSaving = false;
            DialogResult = true;
        }

        private bool CanExecuteSave()
        {
            return !IsSaving && SessionName?.Length > 0 && SessionName?.Length < 128;
        }


        private async Task<bool> SaveModelSession()
        {
            try
            {
                _modelSession.StateName = SessionName;
                var result = await Task.Run(() => _modelSessionService.SaveAsync(_modelSession.SessionId));
                return result is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[SaveModelSession] - {ex.Message}");
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
