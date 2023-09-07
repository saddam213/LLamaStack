using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    public class SessionConfig : ISessionConfig
    {
        public string Model { get; set; }
        public ExecutorType ExecutorType { get; set; } = ExecutorType.Instruct;
        public string Prompt { get; set; }
        public string InputPrefix { get; set; } = "\n\n### Instruction:\n\n";
        public string InputSuffix { get; set; } = "\n\n### Response:\n\n";
        public string AntiPrompt { get; set; } = string.Empty;
        public List<string> AntiPrompts { get; set; } = new List<string>();
        public string OutputFilter { get; set; } = string.Empty;
        public List<string> OutputFilters { get; set; } = new List<string>();
    }
}
