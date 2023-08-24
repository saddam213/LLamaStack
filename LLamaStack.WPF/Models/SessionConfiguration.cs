using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace LLamaStack.WPF
{

    /// <summary>
    /// The sessions configuration object
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Config.ISessionConfig" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class SessionConfiguration : ISessionConfig, INotifyPropertyChanged
    {
        private string _prompt;
        private ModelConfig _selectedModel;
        private string _name = string.Empty;
        private string _antiPrompt = string.Empty;
        private string _outputFilter = string.Empty;
        private LLamaExecutorType _executorType;
        private string _inputPrefix = string.Empty;
        private string _inputSuffix = string.Empty;

        public ModelConfig SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; NotifyPropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }

        public string Model
        {
            get { return _selectedModel?.Name; }
            set { throw new NotSupportedException(); }
        }
        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; NotifyPropertyChanged(); }
        }
        public LLamaExecutorType ExecutorType
        {
            get { return _executorType; }
            set { _executorType = value; NotifyPropertyChanged(); SetDefaultPromptConfig(); }
        }
        public string AntiPrompt
        {
            get { return _antiPrompt; }
            set { _antiPrompt = value; NotifyPropertyChanged(); }
        }
        public string OutputFilter
        {
            get { return _outputFilter; }
            set { _outputFilter = value; NotifyPropertyChanged(); }
        }

        public string InputSuffix
        {
            get { return _inputSuffix; }
            set { _inputSuffix = value; NotifyPropertyChanged(); }
        }

        public string InputPrefix
        {
            get { return _inputPrefix; }
            set { _inputPrefix = value; NotifyPropertyChanged(); }
        }


        public ObservableCollection<SessionHistoryModel> HistoryResponses { get; set; } = new ObservableCollection<SessionHistoryModel>();

        public void PopulateHistory(IEnumerable<SessionHistoryModel> sessionHistory)
        {
            HistoryResponses.Clear();
            if (sessionHistory is null)
                return;

            foreach (var history in sessionHistory)
            {
                HistoryResponses.Add(history);
            }
        }




        private void SetDefaultPromptConfig()
        {
            var defaults = Utils.DefaultPromptConfiguration[_executorType];
            AntiPrompt = defaults.AntiPrompt;
            OutputFilter = defaults.OutputFilter;
            Prompt = defaults.Prompt;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


    }
}
