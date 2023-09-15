using LLama;
using LLama.Abstractions;
using LLamaStack.Core.Common;

namespace LLamaStack.Core.Inference
{
    internal interface IInferenceHandler
    {
        InferenceType Type { get; }
        LLamaContext Context { get; }
        IAsyncEnumerable<TokenData> InferAsync(string text, IInferenceParams inferenceParams = null, CancellationToken token = default);
        Task<InferenceHandlerState> GetState();
        Task SetState(InferenceHandlerState state);
    }
}
