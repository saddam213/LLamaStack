using LLamaStack.Core.Models;

namespace LLamaStack.WebApi.Models
{
    public record InferResponse : InferResponseBase<InferTokenModel>
    {
        public InferResponse(IAsyncEnumerable<InferTokenModel> tokens) 
            : base(tokens)
        {
        }
    }

    public record InferTextResponse : InferResponseBase<string>
    {
        public InferTextResponse(IAsyncEnumerable<string> tokens)
            : base(tokens)
        {
        }
    }

    public record InferResponseBase<T>(IAsyncEnumerable<T> TokensAsync)
    {
        public IEnumerable<T> Tokens => TokensAsync.ToBlockingEnumerable();
    }
}
