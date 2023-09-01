using LLamaStack.Core.Config;
using LLamaStack.WPF.Commands;
using LLamaStack.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LLamaStack.WPF.Views
{
    /// <summary>
    /// Interaction logic for ModelEditorView.xaml
    /// </summary>
    public partial class ModelEditorView : UserControl, INotifyPropertyChanged, ITabView
    {
        private ModelConfiguration _selectedModel;

        public ModelEditorView()
        {
            SaveCommand = new RelayCommand(Save, CanExecuteSave);
            CreateCommand = new RelayCommand(Create, CanExecuteCreate);
            RemoveCommand = new RelayCommand(Remove, CanExecuteRemove);
            Models = new ObservableCollection<ModelConfiguration>();
            InitializeComponent();
        }



        public LLamaStackConfig Configuration
        {
            get { return (LLamaStackConfig)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(LLamaStackConfig), typeof(ModelEditorView));


        public ObservableCollection<ModelConfiguration> Models { get; }

        public ModelConfiguration SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; NotifyPropertyChanged(); }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CreateCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }

        public List<Encoding> Encodings { get; } = new List<Encoding>
        {
            Encoding.UTF8,
            Encoding.UTF32,
            Encoding.ASCII,
            Encoding.Unicode,
            Encoding.Default,
            Encoding.Latin1,
            Encoding.BigEndianUnicode
        };



        private void Create()
        {
            var newItem = ModelConfiguration.From(new ModelConfig());
            newItem.Name = $"New Model {Models.Count(x => x.Name.StartsWith("New Model")) + 1}";
            Models.Add(newItem);
            SelectedModel = newItem;
        }
        private bool CanExecuteCreate()
        {
            return true;
        }


        private void Save()
        {
            Configuration.Models = Models.Select(ModelConfiguration.To).ToList();
            ConfigManager.SaveConfiguration(Configuration);
        }

        private bool CanExecuteSave()
        {
            return !string.IsNullOrEmpty(SelectedModel?.ModelPath);
        }


        private void Remove()
        {
            Models.Remove(SelectedModel);
            SelectedModel = null;
            Save();
            Initialize();
        }

        private bool CanExecuteRemove()
        {
            return true;
        }


        public void Initialize()
        {
            var lastSelected = SelectedModel?.Name;
            Models.Clear();
            foreach (var item in Configuration.Models)
            {
                Models.Add(ModelConfiguration.From(item));
            }

            SelectedModel = Models.FirstOrDefault(x => x.Name == lastSelected)
                         ?? Models.FirstOrDefault();
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
