namespace LLamaStack.Core.Inference
{
    public sealed record TokenData(int Id)
    {
        public float Logit { get; set; }
        public float Probability { get; set; }
        public string Content { get; set; }
        public bool IsChild { get; set; }
    }
}
