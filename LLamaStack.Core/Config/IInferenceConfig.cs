using LLamaStack.Core.Common;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Config
{
    public interface IInferenceConfig
    {
        /// <summary>
        /// Gets or sets the penalty applied to token frequency, affecting token selection during language model inference.
        /// </summary>
        float FrequencyPenalty { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="LLamaStack.Core.Models.LogitBiasModel"/> objects that provide bias information for specific tokens in the language model's vocabulary
        /// </summary>
        List<LogitBiasModel> LogitBias { get; set; }


        /// <summary>
        /// Gets or sets the maximum number of tokens to generate during inference, limiting the length of the generated text
        /// </summary>
        int MaxTokens { get; set; }


        /// <summary>
        /// Gets or sets the type of sampling strategy to use during language model inference (e.g., greedy, top-k, top-p)
        /// </summary>
        SamplerType SamplerType { get; set; }


        /// <summary>
        /// Gets or sets the mirostat eta used to adjust the strength of the Mirostat bias
        /// </summary>
        float MirostatEta { get; set; }


        /// <summary>
        /// Gets or sets the temperature or sensitivity of the Mirostat sampling process
        /// </summary>
        float MirostatTau { get; set; }


        /// <summary>
        /// Determines whether to apply penalty for generating newline characters ("\n") in the generated text.
        /// </summary>
        bool PenalizeNL { get; set; }


        /// <summary>
        /// Gets or sets the penalty applied to token presence in the generated text
        /// </summary>
        float PresencePenalty { get; set; }


        /// <summary>
        /// Gets or sets the number of tokens to repeat at the end of the generated text.
        /// </summary>
        int RepeatLastTokensCount { get; set; }


        /// <summary>
        /// Gets or sets the penalty applied for repeating tokens in the generated text
        /// </summary>
        float RepeatPenalty { get; set; }


        /// <summary>
        /// Gets or sets the temperature parameter for temperature-based sampling. Higher values make output more random, while lower values make it more deterministic
        /// </summary>
        float Temperature { get; set; }


        /// <summary>
        /// Gets or sets the parameter (z) used in the TFS (Top Few Sampling) strategy.
        /// </summary>
        float TfsZ { get; set; }


        /// <summary>
        /// Gets or sets the number of tokens to keep from the input text when generating output.
        /// </summary>
        int TokensKeep { get; set; }


        /// <summary>
        /// Gets or sets the maximum number of tokens to consider during top-k sampling
        /// </summary>
        int TopK { get; set; }


        /// <summary>
        /// Gets or sets the cumulative probability threshold for top-p sampling
        /// </summary>
        float TopP { get; set; }


        /// <summary>
        /// Gets or sets the typicality penalty applied during language model inference.
        /// </summary>
        float TypicalP { get; set; }
    }
}