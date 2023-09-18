namespace LLamaStack.Core.Inference
{
    /// <summary>
    /// Transform the TokenData stream
    /// </summary>
    public interface ITokenStreamTransform
    {

        /// <summary>
        /// Transforms the IAsyncEnumerable<TokenData>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        IAsyncEnumerable<TokenData> TransformAsync(IAsyncEnumerable<TokenData> tokens);
    }
}

