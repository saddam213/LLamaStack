using LLamaStack.WPF.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for Parameters.xaml
    /// </summary>
    public partial class Parameters : UserControl, INotifyPropertyChanged
    {

        /// <summary>Initializes a new instance of the <see cref="Parameters" /> class.</summary>
        public Parameters()
        {
            ResetParametersCommand = new RelayCommand(ResetParameters);
            InitializeComponent();
        }


        /// <summary>Gets the reset parameters command.</summary>
        /// <value>The reset parameters command.</value>
        public ICommand ResetParametersCommand { get; }


        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public InferenceConfiguration Configuration
        {
            get { return (InferenceConfiguration)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }


        /// <summary>
        /// The configuration property
        /// </summary>
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(InferenceConfiguration), typeof(Parameters));


        /// <summary>
        /// Resets the parameters.
        /// </summary>
        private void ResetParameters()
        {
            Configuration = new InferenceConfiguration();
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
