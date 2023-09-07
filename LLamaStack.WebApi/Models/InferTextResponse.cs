namespace LLamaStack.WebApi.Models
{
    public record InferTextResponse : InferResponseBase<string>
    {
        public InferTextResponse(IAsyncEnumerable<string> tokens)
            : base(tokens)
        {
        }
    }
}
