using LLamaStack.Core.Models;

namespace LLamaStack.Core.Services
{
    public interface IModelSessionStateService<T> where T : IEquatable<T>, IComparable<T>
    {
        Task<ModelSessionState<T>> GetAsync(T sessionId);
        Task<IEnumerable<ModelSessionState<T>>> GetAllAsync();
        Task<bool> RemoveAsync(T sessionId);
        Task<ModelSessionState<T>> LoadAsync(T sessionId, CancellationToken cancellationToken = default);
        Task<ModelSessionState<T>> SaveAsync(T sessionId, ModelSession<T> session, CancellationToken cancellationToken = default);
    }
}
