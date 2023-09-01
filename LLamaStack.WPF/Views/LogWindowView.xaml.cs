using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindowView : UserControl, INotifyPropertyChanged, ITabView
    {
        public LogWindowView()
        {
            InitializeComponent();
        }

        public string LogOutput
        {
            get { return (string)GetValue(LogOutputProperty); }
            set { SetValue(LogOutputProperty, value); }
        }

        public static readonly DependencyProperty LogOutputProperty =
            DependencyProperty.Register("LogOutput", typeof(string), typeof(LogWindowView), new PropertyMetadata(string.Empty));

        public void Initialize()
        {

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
