using LLama.Abstractions;
using LLamaStack.Core.Common;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Config
{
    public interface ISessionConfig
    {
        string AntiPrompt { get; set; }
        ExecutorType ExecutorType { get; set; }
        string Model { get; set; }
        string OutputFilter { get; set; }
        string Prompt { get; set; }
        string InputSuffix { get; set; }
        string InputPrefix { get; set; }
    }
}