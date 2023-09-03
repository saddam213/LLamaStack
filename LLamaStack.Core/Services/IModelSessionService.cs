using LLama.Abstractions;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Services
{
    public interface IModelSessionService<T> where T : IEquatable<T>, IComparable<T>
    {
        int InferQueueCount { get; }

        Task<ModelSession<T>> GetAsync(T sessionId);
        Task<IEnumerable<ModelSession<T>>> GetAllAsync();
        Task<bool> CloseAsync(T sessionId);
        Task<bool> CancelAsync(T sessionId);
        Task<ModelSession<T>> CreateAsync(T sessionId, ISessionConfig sessionConfig, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
        Task<string> InferTextAsync(T sessionId, string prompt, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<InferTokenModel> InferAsync(T sessionId, string prompt, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
        Task<string> QueueInferTextAsync(T sessionId, string prompt, IInferenceParams inferenceParams = null, bool saveOnComplete = false, CancellationToken cancellationToken = default);


        Task<ModelSessionState<T>> GetStateAsync(T sessionId);
        Task<IEnumerable<ModelSessionState<T>>> GetStatesAsync();
        Task<bool> RemoveStateAsync(T sessionId);
        Task<ModelSession<T>> LoadStateAsync(T sessionId, CancellationToken cancellationToken = default);
        Task<ModelSessionState<T>> SaveStateAsync(T sessionId, CancellationToken cancellationToken = default);
    }

}
