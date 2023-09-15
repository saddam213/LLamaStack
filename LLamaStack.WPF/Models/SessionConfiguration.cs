using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using LLamaStack.WPF.Models;
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
        private ModelConfiguration _selectedModel;
        private string _name = string.Empty;
        private string _antiPrompt = string.Empty;
        private string _outputFilter = string.Empty;
        private InferenceType _inferenceType;
        private string _inputPrefix = "\n\n### Instruction:\n\n";
        private string _inputSuffix = "\n\n### Response:\n\n";

        public ModelConfiguration SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; NotifyPropertyChanged(); SetDefaultPromptConfig(); }
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
        public InferenceType InferenceType
        {
            get { return _inferenceType; }
            set { _inferenceType = value; NotifyPropertyChanged(); SetDefaultPromptConfig(); }
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

        public List<string> AntiPrompts { get; set; } = new List<string>();

        public List<string> OutputFilters { get; set; } = new List<string>();

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
            var defaults = Utils.DefaultPromptConfiguration[_inferenceType];
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
