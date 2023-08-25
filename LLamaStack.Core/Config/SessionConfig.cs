using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    public class SessionConfig : ISessionConfig
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public ExecutorType ExecutorType { get; set; } = ExecutorType.Instruct;
        public string AntiPrompt { get; set; } = string.Empty;
        public string OutputFilter { get; set; } = string.Empty;
        public string InputSuffix { get; set; } = string.Empty;
        public string InputPrefix { get; set; } = string.Empty;
    }
}
