namespace LLamaStack.Core.Config
{
    public interface IModelConfig
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum context instances.
        /// </summary>
        int MaxInstances { get; set; }

        /// <summary>
        /// Model context size (n_ctx)
        /// </summary>
        uint ContextSize { get; set; }

        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        int MainGpu { get; set; }

        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        int GpuLayerCount { get; set; }

        /// <summary>
        /// Seed for the random number generator (seed)
        /// </summary>
        uint Seed { get; set; }

        /// <summary>
        /// Use f16 instead of f32 for memory kv (memory_f16)
        /// </summary>
        bool UseFp16Memory { get; set; }

        /// <summary>
        /// Use mmap for faster loads (use_mmap)
        /// </summary>
        bool UseMemorymap { get; set; }

        /// <summary>
        /// Use mlock to keep model in memory (use_mlock)
        /// </summary>
        bool UseMemoryLock { get; set; }

        /// <summary>
        /// Compute perplexity over the prompt (perplexity)
        /// </summary>
        bool Perplexity { get; set; }

        /// <summary>
        /// Model path (model)
        /// </summary>
        string ModelPath { get; set; }

        /// <summary>
        /// base model path for the lora adapter (lora_base)
        /// </summary>
        string LoraBase { get; set; }

        /// <summary>
        /// Number of threads (-1 = autodetect) (n_threads)
        /// </summary>
        uint Threads { get; set; }

        /// <summary>
        /// batch size for prompt processing (must be >=32 to use BLAS) (n_batch)
        /// </summary>
        uint BatchSize { get; set; }

        /// <summary>
        /// Whether to use embedding mode. (embedding) Note that if this is set to true, 
        /// The LLamaModel won't produce text response anymore.
        /// </summary>
        bool EmbeddingMode { get; set; }

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        float[] TensorSplits { get; set; }

        /// <summary>
        /// RoPE base frequency
        /// </summary>
        float RopeFrequencyBase { get; set; }

        /// <summary>
        /// RoPE frequency scaling factor
        /// </summary>
        float RopeFrequencyScale { get; set; }

        /// <summary>
        /// Use experimental mul_mat_q kernels
        /// </summary>
        bool MulMatQ { get; set; }

        /// <summary>
        /// The encoding to use for models
        /// </summary>
        string Encoding { get; set; }


        /// <summary>
        /// Gets or sets the batch threads.
        /// </summary>
        uint BatchThreads { get; set; }


        /// <summary>
        /// Gets a value indicating whether vocab only.
        /// </summary>
        bool VocabOnly { get; }
    }
}
