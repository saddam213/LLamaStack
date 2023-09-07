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
}
