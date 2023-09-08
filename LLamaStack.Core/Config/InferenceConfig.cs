using LLamaStack.Core.Common;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Config
{
    /// <summary>
    /// Concrete implemtation of IInferenceConfig
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Config.IInferenceConfig" />
    public class InferenceConfig : IInferenceConfig
    {
        /// <summary>
        /// Gets or sets the number of tokens to keep from the input text when generating output.
        /// </summary>
        public int TokensKeep { get; set; } = 0;


        /// <summary>
        /// Gets or sets the maximum number of tokens to generate during inference, limiting the length of the generated text
        /// </summary>
        public int MaxTokens { get; set; } = -1;

        /// <summary>
        /// Gets or sets the maximum number of tokens to consider during top-k sampling
        /// </summary>
        public int TopK { get; set; } = 40;

        /// <summary>
        /// Gets or sets the cumulative probability threshold for top-p sampling
        /// </summary>
        public float TopP { get; set; } = 0.95f;

        /// <summary>
        /// Gets or sets the parameter (z) used in the TFS (Top Few Sampling) strategy.
        /// </summary>
        public float TfsZ { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the typicality penalty applied during language model inference.
        /// </summary>
        public float TypicalP { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the temperature parameter for temperature-based sampling. Higher values make output more random, while lower values make it more deterministic
        /// </summary>
        public float Temperature { get; set; } = 0.8f;

        /// <summary>
        /// Gets or sets the penalty applied for repeating tokens in the generated text
        /// </summary>
        public float RepeatPenalty { get; set; } = 1.1f;

        /// <summary>
        /// Gets or sets the number of tokens to repeat at the end of the generated text.
        /// </summary>
        public int RepeatLastTokensCount { get; set; } = 64;

        /// <summary>
        /// Gets or sets the penalty applied to token frequency, affecting token selection during language model inference.
        /// </summary>
        public float FrequencyPenalty { get; set; } = .0f;

        /// <summary>
        /// Gets or sets the penalty applied to token presence in the generated text
        /// </summary>
        public float PresencePenalty { get; set; } = .0f;

        /// <summary>
        /// Gets or sets the temperature or sensitivity of the Mirostat sampling process
        /// </summary>
        public float MirostatTau { get; set; } = 5.0f;

        /// <summary>
        /// Gets or sets the mirostat eta used to adjust the strength of the Mirostat bias
        /// </summary>
        public float MirostatEta { get; set; } = 0.1f;

        /// <summary>
        /// Determines whether to apply penalty for generating newline characters ("\n") in the generated text.
        /// </summary>
        public bool PenalizeNL { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of sampling strategy to use during language model inference (e.g., greedy, top-k, top-p)
        /// </summary>
        public SamplerType SamplerType { get; set; } = SamplerType.Default;

        /// <summary>
        /// Gets or sets a list of <see cref="LLamaStack.Core.Models.LogitBiasModel" /> objects that provide bias information for specific tokens in the language model's vocabulary
        /// </summary>
        public List<LogitBiasModel> LogitBias { get; set; } = new List<LogitBiasModel>();
    }
}
