using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LLamaStack.Core.Models
{
    public class SessionHistoryModel : INotifyPropertyChanged
    {
        private string _content;
        private string _signature;
        private bool _isResponse;
        private DateTime _timestamp;

        public string Content
        {
            get { return _content; }
            set { _content = value; NotifyPropertyChanged(); }
        }

        public string Signature
        {
            get { return _signature; }
            set { _signature = value; NotifyPropertyChanged(); }
        }

        public bool IsResponse
        {
            get { return _isResponse; }
            set { _isResponse = value; NotifyPropertyChanged(); }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; NotifyPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }


}
