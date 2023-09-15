namespace LLamaStack.WebApi.Models
{
    public record ModelInfo(string Name)
    {
        public int ContextSize { get; set; }
        public int BatchSize { get; set; }
        public int MaxInstances { get; set; }
        public string Encoding { get; set; }
    }
}
