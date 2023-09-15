using LLama.Abstractions;
using LLamaStack.Core.Common;

namespace LLamaStack.Core.Inference
{
    public interface IInferenceHandler
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        InferenceType Type { get; }

        /// <summary>
        /// Runs inference on the current LLamaStackContext
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="inferenceParams">The inference configuration</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Streaming async result of <see cref="LLamaStack.Core.Inference.TokenData" /></returns>
        IAsyncEnumerable<TokenData> InferAsync(string text, IInferenceParams inferenceParams = null, CancellationToken token = default);


        /// <summary>
        /// Gets the handlers state.
        /// </summary>
        Task<InferenceHandlerState> GetStateAsync();


        /// <summary>
        /// Sets the handlers state.
        /// </summary>
        /// <param name="state">The state.</param>
        Task SetStateAsync(InferenceHandlerState state);
    }
}
