using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    /// <summary>
    /// Interface for Session configurations
    /// </summary>
    public interface ISessionConfig
    {
        /// <summary>
        /// Gets or sets the model name to open the session on
        /// </summary>
        string Model { get; set; }

        /// <summary>
        /// Gets or sets the type of inference
        /// </summary>
        InferenceType InferenceType { get; set; }

        /// <summary>
        /// Gets or sets the initial prompt to start the session with.
        /// </summary>
        string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the input prefix for Instruct mode.
        /// </summary>
        string InputPrefix { get; set; }

        /// <summary>
        /// Gets or sets the input suffix for Instruct mode.
        /// </summary>
        string InputSuffix { get; set; }

        /// <summary>
        /// Gets or sets one or more anti-prompt words as CSV. (Combined with AntiPrompts)
        /// </summary>
        string AntiPrompt { get; set; }

        /// <summary>
        /// Gets or sets a list of anti-prompt words. (Combined with AntiPrompt)
        /// </summary>
        public List<string> AntiPrompts { get; set; }

        /// <summary>
        /// Gets or sets a list of words to remove from the output as CSV. (Combined with OutputFilters)
        /// </summary>
        string OutputFilter { get; set; }

        /// <summary>
        /// Gets or sets a list of words to remove from the output, (Combined with OutputFilter)
        /// </summary>
        public List<string> OutputFilters { get; set; }
    }
}