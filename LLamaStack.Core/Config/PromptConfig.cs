namespace LLamaStack.Core.Config
{
    public class PromptConfig : IPromptConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Prompt { get; set; }
        public List<string> AntiPrompt { get; set; }
        public List<string> OutputFilter { get; set; }
    }
}
