using LLamaStack.Core.Common;

namespace LLamaStack.Core.Config
{
    public interface ISessionConfig
    {
        string Model { get; set; }
        ExecutorType ExecutorType { get; set; }
        string Prompt { get; set; }
        string InputPrefix { get; set; }
        string InputSuffix { get; set; }
        string AntiPrompt { get; set; }
        public List<string> AntiPrompts { get; set; }
        string OutputFilter { get; set; }
        public List<string> OutputFilters { get; set; }
    }
}