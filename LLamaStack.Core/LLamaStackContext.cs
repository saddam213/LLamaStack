using LLama;
using LLama.Abstractions;
using LLama.Native;
using LLamaStack.Core.Extensions;
using LLamaStack.Core.Inference;
using System.Text;

namespace LLamaStack.Core
{
    /// <summary>
    /// Wrapper class for LLamaSharp LLamaContext
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LLamaStackContext : IDisposable
    {
        private readonly LLamaContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaStackContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LLamaStackContext(LLamaContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Gets the LLamaSharp context.
        /// </summary>
        public LLamaContext LLamaContext => _context;


        /// <summary>
        /// Gets the size of the context.
        /// </summary>
        public int ContextSize => _context.ContextSize;


        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void LoadState(string filename)
        {
            _context.LoadState(filename);
        }


        /// <summary>
        /// Loads the state asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public async Task LoadStateAsync(string filename)
        {
            await Task.Run(() => LoadState(filename));
        }


        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveState(string filename)
        {
            _context.SaveState(filename);
        }


        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public async Task SaveStateAsync(string filename)
        {
            await Task.Run(() => SaveState(filename));
        }


        public TokenData GetTokenData(LLamaTokenDataArray tokenDataArray, int id)
        {
            // TODO: are all samplers sorted? if not we need to do a binary serach using id
            var tokenDataSpan = tokenDataArray.data[..1].Span;
            if (tokenDataSpan.Length == 0)
                throw new InvalidOperationException("The input sequence is empty.");

            var tokenData = tokenDataSpan[0];
            return new TokenData(tokenData.id)
            {
                Logit = tokenData.logit,
                Probability = tokenData.p
            };
        }


        /// <summary>
        /// Apply the penalty for the tokens.
        /// </summary>
        /// <param name="lastTokens">The last tokens.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <returns></returns>
        public LLamaTokenDataArray ApplyPenalty(IEnumerable<TokenData> lastTokens, IInferenceParams inferenceParams)
        {
            var repeatLastN = inferenceParams.RepeatLastTokensCount < 0
                ? _context.ContextSize
                : inferenceParams.RepeatLastTokensCount;

            return _context.ApplyPenalty
            (
                lastTokens.ToTokenIds(),
                inferenceParams.LogitBias,
                repeatLastN,
                inferenceParams.RepeatPenalty,
                inferenceParams.FrequencyPenalty,
                inferenceParams.PresencePenalty,
                inferenceParams.PenalizeNL
            );
        }


        /// <summary>
        /// Perform the sampling.
        /// </summary>
        /// <param name="tokenDataArray">The token data array.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="mirostatMu">The mirostat mu.</param>
        /// <returns></returns>
        public int Sample(LLamaTokenDataArray tokenDataArray, IInferenceParams inferenceParams, ref float? mirostatMu)
        {
            return _context.Sample
            (
                tokenDataArray,
                ref mirostatMu,
                inferenceParams.Temperature,
                inferenceParams.Mirostat,
                inferenceParams.MirostatTau,
                inferenceParams.MirostatEta,
                inferenceParams.TopK,
                inferenceParams.TopP,
                inferenceParams.TfsZ,
                inferenceParams.TypicalP,
                inferenceParams.Grammar,
                inferenceParams.MinP
            );
        }


        /// <summary>
        /// Tokenizes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="addBos">if set to <c>true</c> [add bos].</param>
        /// <returns></returns>
        private IEnumerable<TokenData> TokenizeText(string text, bool addBos)
        {
            return _context.Tokenize(text, addBos)
                .Select(x => new TokenData(x));
        }


        /// <summary>
        /// Tokenizes the text to list.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="addBos">if set to <c>true</c> [add bos].</param>
        /// <returns></returns>
        public List<TokenData> TokenizeTextToList(string text, bool addBos)
        {
            return TokenizeText(text, addBos).ToList();
        }


        /// <summary>
        /// Tokenizes the text to array.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="addBos">if set to <c>true</c> [add bos].</param>
        /// <returns></returns>
        public TokenData[] TokenizeTextToArray(string text, bool addBos)
        {
            return TokenizeText(text, addBos).ToArray();
        }


        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="pastTokensCount">The past tokens count.</param>
        /// <returns></returns>
        public Task<int> EvalAsync(IEnumerable<TokenData> tokens, int pastTokensCount)
        {
            return Task.Run(() => _context.Eval(tokens.ToTokenIds(), pastTokensCount));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
