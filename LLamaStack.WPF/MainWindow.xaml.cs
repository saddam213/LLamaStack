using LLamaStack.Core.Config;
using LLamaStack.WPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Configuration;
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


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(ILogger<MainWindow> logger, LLamaStackConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Configuration = configuration;
            InitializeComponent();
        }




        public LLamaStackConfig Configuration
        {
            get { return (LLamaStackConfig)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Configuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(LLamaStackConfig), typeof(MainWindow));



        //public LLamaStackConfig Configuration
        //{
        //    get { return _configuration; }
        //    set { _configuration = value; NotifyPropertyChanged(); }
        //}

        private ITabView _selctedTab;

        public ITabView SelectedTab
        {
            get { return _selctedTab; }
            set { _selctedTab = value; _selctedTab?.Initialize(); NotifyPropertyChanged(); }
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