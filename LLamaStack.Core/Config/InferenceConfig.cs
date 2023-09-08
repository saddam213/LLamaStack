using LLamaStack.Core.Common;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Config
{
    public class InferenceConfig : IInferenceConfig
    {
        public int TokensKeep { get; set; } = 0;
        public int MaxTokens { get; set; } = -1;
        public int TopK { get; set; } = 40;
        public float TopP { get; set; } = 0.95f;
        public float TfsZ { get; set; } = 1.0f;
        public float TypicalP { get; set; } = 1.0f;
        public float Temperature { get; set; } = 0.8f;
        public float RepeatPenalty { get; set; } = 1.1f;
        public int RepeatLastTokensCount { get; set; } = 64;
        public float FrequencyPenalty { get; set; } = .0f;
        public float PresencePenalty { get; set; } = .0f;
        public float MirostatTau { get; set; } = 5.0f;
        public float MirostatEta { get; set; } = 0.1f;
        public bool PenalizeNL { get; set; } = true;
        public SamplerType SamplerType { get; set; } = SamplerType.Default;
        public List<LogitBiasModel> LogitBias { get; set; } = new List<LogitBiasModel>();
    }
}
