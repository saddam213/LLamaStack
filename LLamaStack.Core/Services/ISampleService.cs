using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core.Inference;

namespace LLamaStack.Core.Services
{
    public interface ISampleService
    {
        float? MirostatMu { get; set; }

        Task<TokenData> SampleAsync(IInferenceParams inferenceParams, IEnumerable<TokenData> lastTokens);
    }
}