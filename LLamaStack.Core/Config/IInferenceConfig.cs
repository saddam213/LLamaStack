using LLamaStack.Core.Common;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Config
{
    public interface IInferenceConfig
    {
        float FrequencyPenalty { get; set; }
        List<LogitBiasModel> LogitBias { get; set; }
        int MaxTokens { get; set; }
        SamplerType SamplerType { get; set; }
        float MirostatEta { get; set; }
        float MirostatTau { get; set; }
        bool PenalizeNL { get; set; }
        float PresencePenalty { get; set; }
        int RepeatLastTokensCount { get; set; }
        float RepeatPenalty { get; set; }
        float Temperature { get; set; }
        float TfsZ { get; set; }
        int TokensKeep { get; set; }
        int TopK { get; set; }
        float TopP { get; set; }
        float TypicalP { get; set; }
    }
}