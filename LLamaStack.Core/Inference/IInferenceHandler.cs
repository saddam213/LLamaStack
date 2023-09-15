using LLama.Abstractions;
using LLamaStack.Core.Common;

namespace LLamaStack.Core.Inference
{
    public interface IInferenceHandler
    {
        InferenceType Type { get; }
        IAsyncEnumerable<TokenData> InferAsync(string text, IInferenceParams inferenceParams = null, CancellationToken token = default);
        Task<InferenceHandlerState> GetStateAsync();
        Task SetStateAsync(InferenceHandlerState state);
    }
}
