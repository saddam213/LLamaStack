using LLamaStack.Core;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using LLamaStack.Core.Services;
using LLamaStack.WPF.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for SessionLoadView.xaml
    /// </summary>
    public partial class SessionLoadView : Window, INotifyPropertyChanged
    {
        private readonly LLamaStackConfig _configuration;
        private readonly ILogger<SessionLoadView> _logger;
        private readonly IModelSessionService<Guid> _modelSessionService;

        private bool _isLoading = false;
        private ModelSession<Guid> _selectedModelSession;
        private ModelSessionState<Guid> _selectedModelSessionState;

        public SessionLoadView(ILogger<SessionLoadView> logger, LLamaStackConfig configuration, IModelSessionService<Guid> modelSessionService)
        {
            _logger = logger;
            _configuration = configuration;
            _modelSessionService = modelSessionService;
            LoadCommand = new AsyncRelayCommand(Load, CanExecuteLoad);
            CancelCommand = new AsyncRelayCommand(Cancel, CanExecuteCancel);
            DeleteSessionCommand = new AsyncRelayCommand<Guid>(DeleteSession, CanExecuteDeleteSession);
            InitializeComponent();
        }

        public AsyncRelayCommand LoadCommand { get; }
        public AsyncRelayCommand CancelCommand { get; }
        public AsyncRelayCommand<Guid> DeleteSessionCommand { get; }
        public ModelSession<Guid> GetSeletedModel() => _selectedModelSession;
        public ObservableCollection<ModelSessionState<Guid>> SavedSessions { get; set; } = new ObservableCollection<ModelSessionState<Guid>>();

        public ModelSessionState<Guid> SelectedSession
        {
            get { return _selectedModelSessionState; }
            set { _selectedModelSessionState = value; NotifyPropertyChanged(); }
        }


        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged(); }
        }





        private Task Cancel()
        {
            DialogResult = false;
            return Task.CompletedTask;
        }


        private bool CanExecuteCancel()
        {
            return !IsLoading;
        }


        private async Task Load()
        {
            IsLoading = true;
            try
            {
                _selectedModelSession = await Task.Run(() => _modelSessionService.LoadStateAsync(_selectedModelSessionState.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[LoadModelSession] - {ex.Message}");
            }

            IsLoading = false;
            DialogResult = _selectedModelSession is not null;
        }


        private bool CanExecuteLoad()
        {
            return !IsLoading && SelectedSession is not null;
        }


        private async Task DeleteSession(Guid sessionId)
        {
            IsLoading = true;
            try
            {
                // Delete Session
                if(await _modelSessionService.RemoveStateAsync(sessionId))
                {
                    // Remove from UI
                    SavedSessions.Remove(SavedSessions.FirstOrDefault(x => x.Id == sessionId));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[DeleteSession] - {ex.Message}");
            }
            IsLoading = false;
        }

        private bool CanExecuteDeleteSession(Guid guid)
        {
            return !IsLoading;
        }


        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SavedSessions.Clear();
            var modelStates = await _modelSessionService.GetStatesAsync();
            foreach (var item in modelStates.OrderByDescending(x => x.Created))
            {
                SavedSessions.Add(item);
            }
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
