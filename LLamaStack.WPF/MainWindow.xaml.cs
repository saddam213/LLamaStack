using LLamaStack.Core.Config;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace LLamaStack.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ILogger<MainWindow> _logger;
        private LLamaStackConfig _configuration;
        private string _outputLog;
        private ITabView _selectedTab;


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(ILogger<MainWindow> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            Configuration = configuration;
            InitializeComponent();
        }


        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public LLamaStackConfig Configuration
        {
            get { return _configuration; }
            set { _configuration = value; NotifyPropertyChanged(); }
        }


        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public ITabView SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                _selectedTab?.Initialize();
                NotifyPropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets the output log.
        /// </summary>
        public string OutputLog
        {
            get { return _outputLog; }
            set { _outputLog = value; NotifyPropertyChanged(); }
        }


        /// <summary>
        /// Updates the output log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void UpdateOutputLog(string message)
        {
            OutputLog += message;
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