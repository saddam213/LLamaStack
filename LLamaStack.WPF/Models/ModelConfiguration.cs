using LLamaStack.Core.Config;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LLamaStack.WPF.Models
{
    public class ModelConfiguration : IModelConfig, INotifyPropertyChanged
    {
        private int _maxInstances;
        private string _name;
        private string _encoding;
        private bool _mulMatQ;
        private float _ropeFrequencyScale;
        private float _ropeFrequencyBase;
        private float _rmsNormEpsilon;
        private int _groupedQueryAttention;
        private float[] _tensorSplits;
        private bool _embeddingMode;
        private bool _convertEosToNewLine;
        private int _batchSize;
        private int _threads;
        private string _loraBase;
        private string _loraAdapter;
        private string _modelAlias;
        private string _modelPath;
        private bool _perplexity;
        private bool _useMemoryLock;
        private bool _useMemorymap;
        private bool _useFp16Memory;
        private int _seed;
        private int _gpuLayerCount;
        private bool _lowVram;
        private int _mainGpu;
        private int _contextSize;


        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }
        public int MaxInstances
        {
            get { return _maxInstances; }
            set { _maxInstances = value; NotifyPropertyChanged(); }
        }

        public int ContextSize
        {
            get { return _contextSize; }
            set { _contextSize = value; NotifyPropertyChanged(); }
        }
        public int MainGpu
        {
            get { return _mainGpu; }
            set { _mainGpu = value; NotifyPropertyChanged(); }
        }
        public bool LowVram
        {
            get { return _lowVram; }
            set { _lowVram = value; NotifyPropertyChanged(); }
        }
        public int GpuLayerCount
        {
            get { return _gpuLayerCount; }
            set { _gpuLayerCount = value; NotifyPropertyChanged(); }
        }
        public int Seed
        {
            get { return _seed; }
            set { _seed = value; NotifyPropertyChanged(); }
        }
        public bool UseFp16Memory
        {
            get { return _useFp16Memory; }
            set { _useFp16Memory = value; NotifyPropertyChanged(); }
        }
        public bool UseMemorymap
        {
            get { return _useMemorymap; }
            set { _useMemorymap = value; NotifyPropertyChanged(); }
        }
        public bool UseMemoryLock
        {
            get { return _useMemoryLock; }
            set { _useMemoryLock = value; NotifyPropertyChanged(); }
        }
        public bool Perplexity
        {
            get { return _perplexity; }
            set { _perplexity = value; NotifyPropertyChanged(); }
        }
        public string ModelPath
        {
            get { return _modelPath; }
            set { _modelPath = value; NotifyPropertyChanged(); }
        }
        public string ModelAlias
        {
            get { return _modelAlias; }
            set { _modelAlias = value; NotifyPropertyChanged(); }
        }
        public string LoraAdapter
        {
            get { return _loraAdapter; }
            set { _loraAdapter = value; NotifyPropertyChanged(); }
        }
        public string LoraBase
        {
            get { return _loraBase; }
            set { _loraBase = value; NotifyPropertyChanged(); }
        }
        public int Threads
        {
            get { return _threads; }
            set { _threads = value; NotifyPropertyChanged(); }
        }
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; NotifyPropertyChanged(); }
        }
        public bool ConvertEosToNewLine
        {
            get { return _convertEosToNewLine; }
            set { _convertEosToNewLine = value; NotifyPropertyChanged(); }
        }
        public bool EmbeddingMode
        {
            get { return _embeddingMode; }
            set { _embeddingMode = value; NotifyPropertyChanged(); }
        }
        public float[] TensorSplits
        {
            get { return _tensorSplits; }
            set { _tensorSplits = value; NotifyPropertyChanged(); }
        }
        public int GroupedQueryAttention
        {
            get { return _groupedQueryAttention; }
            set { _groupedQueryAttention = value; NotifyPropertyChanged(); }
        }
        public float RmsNormEpsilon
        {
            get { return _rmsNormEpsilon; }
            set { _rmsNormEpsilon = value; NotifyPropertyChanged(); }
        }
        public float RopeFrequencyBase
        {
            get { return _ropeFrequencyBase; }
            set { _ropeFrequencyBase = value; NotifyPropertyChanged(); }
        }
        public float RopeFrequencyScale
        {
            get { return _ropeFrequencyScale; }
            set { _ropeFrequencyScale = value; NotifyPropertyChanged(); }
        }
        public bool MulMatQ
        {
            get { return _mulMatQ; }
            set { _mulMatQ = value; NotifyPropertyChanged(); }
        }
        public string Encoding
        {
            get { return _encoding; }
            set { _encoding = value; NotifyPropertyChanged(); }
        }

        public static ModelConfiguration From(ModelConfig config)
        {
            return new ModelConfiguration
            {
                BatchSize = config.BatchSize,
                ContextSize = config.ContextSize,
                ConvertEosToNewLine = config.ConvertEosToNewLine,
                EmbeddingMode = config.EmbeddingMode,
                Encoding = config.Encoding,
                GpuLayerCount = config.GpuLayerCount,
                GroupedQueryAttention = config.GroupedQueryAttention,
                LoraAdapter = config.LoraAdapter,
                LoraBase = config.LoraBase,
                LowVram = config.LowVram,
                MainGpu = config.MainGpu,
                MaxInstances = config.MaxInstances,
                ModelAlias = config.ModelAlias,
                ModelPath = config.ModelPath,
                MulMatQ = config.MulMatQ,
                Name = config.Name,
                Perplexity = config.Perplexity,
                RmsNormEpsilon = config.RmsNormEpsilon,
                RopeFrequencyBase = config.RopeFrequencyBase,
                RopeFrequencyScale = config.RopeFrequencyScale,
                Seed = config.Seed,
                TensorSplits = config.TensorSplits,
                Threads = config.Threads,
                UseFp16Memory = config.UseFp16Memory,
                UseMemoryLock = config.UseMemoryLock,
                UseMemorymap = config.UseMemorymap
            };
        }

        public static ModelConfig To(ModelConfiguration config)
        {
            return new ModelConfig
            {
                BatchSize = config.BatchSize,
                ContextSize = config.ContextSize,
                ConvertEosToNewLine = config.ConvertEosToNewLine,
                EmbeddingMode = config.EmbeddingMode,
                Encoding = config.Encoding,
                GpuLayerCount = config.GpuLayerCount,
                GroupedQueryAttention = config.GroupedQueryAttention,
                LoraAdapter = config.LoraAdapter,
                LoraBase = config.LoraBase,
                LowVram = config.LowVram,
                MainGpu = config.MainGpu,
                MaxInstances = config.MaxInstances,
                ModelAlias = config.ModelAlias,
                ModelPath = config.ModelPath,
                MulMatQ = config.MulMatQ,
                Name = config.Name,
                Perplexity = config.Perplexity,
                RmsNormEpsilon = config.RmsNormEpsilon,
                RopeFrequencyBase = config.RopeFrequencyBase,
                RopeFrequencyScale = config.RopeFrequencyScale,
                Seed = config.Seed,
                TensorSplits = config.TensorSplits,
                Threads = config.Threads,
                UseFp16Memory = config.UseFp16Memory,
                UseMemoryLock = config.UseMemoryLock,
                UseMemorymap = config.UseMemorymap
            };
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
