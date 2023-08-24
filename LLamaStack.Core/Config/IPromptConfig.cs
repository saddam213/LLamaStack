namespace LLamaStack.Core.Config
{
    public interface IPromptConfig
    {
        List<string> AntiPrompt { get; set; }
        string Name { get; set; }
        List<string> OutputFilter { get; set; }
        string Path { get; set; }
        string Prompt { get; set; }
    }
}