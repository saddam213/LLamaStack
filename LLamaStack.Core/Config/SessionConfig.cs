using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    /// <summary>
    /// Concrete implemetation of ISessionConfig
    /// </summary>
    /// <seealso cref="LLamaStack.Core.Config.ISessionConfig" />
    public class SessionConfig : ISessionConfig
    {

        /// <summary>
        /// Gets or sets the model name to open the session on
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the type of the executor to use for inference.
        /// </summary>
        public ExecutorType ExecutorType { get; set; } = ExecutorType.Instruct;

        /// <summary>
        /// Gets or sets the initial prompt to start the session with.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the input prefix for Instruct executors.
        /// </summary>
        public string InputPrefix { get; set; } = "\n\n### Instruction:\n\n";

        /// <summary>
        /// Gets or sets the input suffix for Instruct executors.
        /// </summary>
        public string InputSuffix { get; set; } = "\n\n### Response:\n\n";

        /// <summary>
        /// Gets or sets one or more anti-prompt words as CSV. (Combined with AntiPrompts)
        /// </summary>
        public string AntiPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a list of anti-prompt words. (Combined with AntiPrompt)
        /// </summary>
        public List<string> AntiPrompts { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a list of words to remove from the output as CSV. (Combined with OutputFilters)
        /// </summary>
        public string OutputFilter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a list of words to remove from the output, (Combined with OutputFilter)
        /// </summary>
        public List<string> OutputFilters { get; set; } = new List<string>();
    }
}
