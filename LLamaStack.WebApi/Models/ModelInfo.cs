namespace LLamaStack.WebApi.Models
{
    public record ModelInfo(string Name)
    {
        public uint ContextSize { get; set; }
        public uint BatchSize { get; set; }
        public int MaxInstances { get; set; }
        public string Encoding { get; set; }
    }
}
