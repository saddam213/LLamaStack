namespace LLamaStack.Core.Inference
{
    public interface ITokenStreamTransform
    {
        IAsyncEnumerable<TokenData> TransformAsync(IAsyncEnumerable<TokenData> tokens);
    }
}

