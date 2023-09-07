using LLama.Common;
using LLamaStack.WebApi.Models;

namespace LLamaStack.WebApi
{
    public static class Extensions
    {
        public static InferenceParams ToInferenceParams(this InferRequestBase inferRequest)
        {
            return new InferenceParams
            {
                FrequencyPenalty = inferRequest.FrequencyPenalty,
                MaxTokens = inferRequest.MaxTokens,
                Mirostat = inferRequest.Mirostat,
                MirostatEta = inferRequest.MirostatEta,
                MirostatTau = inferRequest.MirostatTau,
                PenalizeNL = inferRequest.PenalizeNL,
                PresencePenalty = inferRequest.PresencePenalty,
                RepeatLastTokensCount = inferRequest.RepeatLastTokensCount,
                RepeatPenalty = inferRequest.RepeatPenalty,
                Temperature = inferRequest.Temperature,
                TfsZ = inferRequest.TfsZ,
                TokensKeep = inferRequest.TokensKeep,
                TopK = inferRequest.TopK,
                TopP = inferRequest.TopP,
                TypicalP = inferRequest.TypicalP,
                LogitBias = inferRequest.LogitBias?.ToDictionary(k => k.TokenId, v => v.Bias)
            };
        }
    }
}
