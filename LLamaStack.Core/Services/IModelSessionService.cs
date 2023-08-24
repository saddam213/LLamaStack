using LLama.Abstractions;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;

namespace LLamaStack.Core.Services
{
    public interface IModelSessionService<T> where T : IEquatable<T>
    {
        Task<ModelSessionState<T>> GetAsync(T sessionId);
        Task<IEnumerable<ModelSessionState<T>>> GetAllAsync();

        Task<bool> CloseAsync(T sessionId);
        Task<bool> CancelAsync(T sessionId);
        Task<ModelSession<T>> CreateAsync(T sessionId, ISessionConfig sessionConfig, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<InferTokenModel> InferAsync(T sessionId, string prompt, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(T sessionId);
        Task<ModelSession<T>> LoadAsync(T sessionId, CancellationToken cancellationToken = default);
        Task<ModelSessionState<T>> SaveAsync(T sessionId, CancellationToken cancellationToken = default);
    }

}
