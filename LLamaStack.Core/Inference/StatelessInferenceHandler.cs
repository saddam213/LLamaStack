using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core.Common;
using System.Text;

namespace LLamaStack.Core.Inference
{
    internal class StatelessInferenceHandler : IInferenceHandler
    {
        private readonly LLamaWeights _weights;
        private readonly IModelParams _params;

        public StatelessInferenceHandler(LLamaWeights weights, IModelParams @params)
        {
            _weights = weights;
            _params = @params;
        }

        public LLamaContext Context { get; private set; }

        public InferenceType Type => InferenceType.Stateless;

        /// <inheritdoc />
        public async IAsyncEnumerable<TokenData> InferAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            using (Context = _weights.CreateContext(_params))
            {
                if (inferenceParams != null)
                {
                    if (inferenceParams.TokensKeep > Context.ContextSize)
                        throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({Context.ContextSize})");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var antiprompts = inferenceParams?.AntiPrompts.ToArray() ?? Array.Empty<string>();
                var n_past = 1;
                inferenceParams ??= new InferenceParams();

                var lastTokens = new List<TokenData>(inferenceParams.RepeatLastTokensCount);
                for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                    lastTokens.Add(new TokenData(0));

                var tokens = Context.TokenizeTextToList(text, true);
                var n_prompt_tokens = tokens.Count;

                await Context.Eval(tokens, n_past);

                lastTokens.AddRange(tokens);
                n_past += n_prompt_tokens;

                var mu = (float?)null;
                var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
                for (var i = 0; i < max_tokens; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                 
                    var tokenDataArray = Context.ApplyPenalty(lastTokens, inferenceParams);

                    var id = Context.Sample(tokenDataArray, inferenceParams, ref mu);

                    var tokenData = tokenDataArray.GetTokenData(Context, id);

                    lastTokens.Add(tokenData);

                    yield return tokenData;

                    tokens.Clear();
                    tokens.Add(tokenData);

                    if (EndsWithAntiprompt(lastTokens, antiprompts))
                        break;

                    // when run out of context
                    // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L433
                    if (n_past + tokens.Count > Context.ContextSize)
                    {
                        var n_left = n_past - inferenceParams.TokensKeep;

                        n_past = Math.Max(1, inferenceParams.TokensKeep);

                        tokens.Clear();
                        tokens.AddRange(lastTokens.Skip(lastTokens.Count - n_left / 2).Take(n_left / 2));
                    }

                    n_past = await Context.Eval(tokens, n_past);
                }
            }
        }


        /// <summary>
        /// Check if the given tokens list ends with any of the antiprompts
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="antiprompts"></param>
        /// <returns></returns>
        private bool EndsWithAntiprompt(IReadOnlyList<TokenData> tokens, IReadOnlyList<string> antiprompts)
        {
            if (antiprompts.Count == 0 || tokens.Count == 0)
                return false;

            var builder = new StringBuilder();
            foreach (var token in tokens)
                builder.Append(token.Content);

            var last_output = builder.ToString();

            foreach (var antiprompt in antiprompts)
            {
                if (last_output.EndsWith(antiprompt))
                    return true;
            }

            return false;
        }


        public Task<InferenceHandlerState> GetState()
        {
            return Task.FromResult(default(InferenceHandlerState));
        }

        public Task SetState(InferenceHandlerState state)
        {
            return Task.CompletedTask;
        }
    }
}
