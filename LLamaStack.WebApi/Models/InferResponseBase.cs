namespace LLamaStack.WebApi.Models
{
    public record InferResponseBase<T>(IAsyncEnumerable<T> TokensAsync)
    {
        public IEnumerable<T> Tokens => TokensAsync.ToBlockingEnumerable();
    }
}
