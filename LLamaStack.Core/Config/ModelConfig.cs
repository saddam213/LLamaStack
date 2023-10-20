
namespace LLamaStack.Core.Config
{

    /// <summary>
    /// Concrete implentation of IModelConfig
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Config.IModelConfig" />
    public class ModelConfig : IModelConfig
    {

        /// <summary>
        /// Gets or sets the maximum context instances.
        /// </summary>
        public int MaxInstances { get; set; } = -1;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = "unknown";

        /// <summary>
        /// Model context size (n_ctx)
        /// </summary>
        public uint ContextSize { get; set; } = 512;

        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        public int MainGpu { get; set; } = 0;


        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        public int GpuLayerCount { get; set; } = 20;

        /// <summary>
        /// Seed for the random number generator (seed)
        /// </summary>
        public uint Seed { get; set; } = 1686349486;

        /// <summary>
        /// Use f16 instead of f32 for memory kv (memory_f16)
        /// </summary>
        public bool UseFp16Memory { get; set; } = true;

        /// <summary>
        /// Use mmap for faster loads (use_mmap)
        /// </summary>
        public bool UseMemorymap { get; set; } = true;

        /// <summary>
        /// Use mlock to keep model in memory (use_mlock)
        /// </summary>
        public bool UseMemoryLock { get; set; } = false;

        /// <summary>
        /// Compute perplexity over the prompt (perplexity)
        /// </summary>
        public bool Perplexity { get; set; } = false;

        /// <summary>
        /// Model path (model)
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// base model path for the lora adapter (lora_base)
        /// </summary>
        public string LoraBase { get; set; } = string.Empty;

        /// <summary>
        /// Number of threads (-1 = autodetect) (n_threads)
        /// </summary>
        public uint Threads { get; set; } = 0;

        /// <summary>
        /// batch size for prompt processing (must be &gt;=32 to use BLAS) (n_batch)
        /// </summary>
        public uint BatchSize { get; set; } = 512;

        /// <summary>
        /// Whether to use embedding mode. (embedding) Note that if this is set to true,
        /// The LLamaModel won't produce text response anymore.
        /// </summary>
        public bool EmbeddingMode { get; set; } = false;

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        public float[] TensorSplits { get; set; } = new float[] { 0 };

        /// <summary>
        /// RoPE base frequency
        /// </summary>
        public float RopeFrequencyBase { get; set; } = 0f;

        /// <summary>
        /// RoPE frequency scaling factor
        /// </summary>
        public float RopeFrequencyScale { get; set; } = 0f;

        /// <summary>
        /// Use experimental mul_mat_q kernels
        /// </summary>
        public bool MulMatQ { get; set; }

        /// <summary>
        /// The encoding to use for models
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";


        /// <summary>
        /// Gets or sets the batch threads.
        /// </summary>
        public uint BatchThreads { get; set; } = 0;


        /// <summary>
        /// Gets a value indicating whether vocab only.
        /// </summary>
        public bool VocabOnly { get; set; } = false;
    }
}
