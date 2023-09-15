using LLamaStack.Core.Common;

namespace LLamaStack.Core.Inference
{
    public class InferenceHandlerState
    {
        public int PastTokensCount { get; set; }
        public int ConsumedTokensCount { get; set; }
        public List<TokenData> Embeds { get; set; }
        public List<TokenData> EmbedInps { get; set; }
        public TokenData[] LastTokens { get; set; }
        public int LastTokensCapacity { get; set; }
        public float? MirostatMu { get; set; }
        public bool IsPromptRun { get; set; }
        public TokenData[] InputPrefixTokens { get; set; }
        public TokenData[] InputSuffixTokens { get; set; }
        public InferenceType InferenceType { get; set; }
    }
}
