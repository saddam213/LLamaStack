using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core.Common;
using LLamaStack.Core.Services;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaStack.Core.Inference
{
    public sealed class StatelessInferenceHandler<T> : IInferenceHandler
    {
        private readonly LLamaStackModel<T> _model;
        private readonly IContextParams _params;

        public StatelessInferenceHandler(LLamaWeights weights, IContextParams @params)
        /// Initializes a new instance of the <see cref="StatelessInferenceHandler{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public StatelessInferenceHandler(LLamaStackModel<T> model)
        {
            _weights = weights;
            _params = @params;
        }

        public LLamaContext Context { get; private set; }

        /// <summary>
        /// Gets the InferenceType.
        /// </summary>
        public InferenceType Type => InferenceType.Stateless;


        /// <summary>
        /// Execute the inference.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">inferenceParams - TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({context.ContextSize})</exception>
        public async IAsyncEnumerable<TokenData> InferAsync(string text, IInferenceParams inferenceParams = null, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            using (var context = await _model.CreateContext(default))
            {

                if (inferenceParams != null)
                {
                    if (inferenceParams.TokensKeep > context.ContextSize)
                        throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({context.ContextSize})");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var antiprompts = inferenceParams?.AntiPrompts.ToArray() ?? Array.Empty<string>();
                var n_past = 1;
                inferenceParams ??= new InferenceParams();

                var lastTokens = new List<TokenData>(inferenceParams.RepeatLastTokensCount);
                for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                    lastTokens.Add(new TokenData(0));

                var tokens = context.TokenizeTextToList(text, true);
                var n_prompt_tokens = tokens.Count;

                await context.EvalAsync(tokens, n_past);

                lastTokens.AddRange(tokens);
                n_past += n_prompt_tokens;

                var sampleService = new SampleService(context);
                var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
                for (var i = 0; i < max_tokens; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var tokenData = await sampleService.SampleAsync(inferenceParams, lastTokens);

                    lastTokens.Add(tokenData);

                    yield return tokenData;

                    tokens.Clear();
                    tokens.Add(tokenData);

                    if (EndsWithAntiprompt(lastTokens, antiprompts))
                        break;

                    // when run out of context
                    // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L433
                    if (n_past + tokens.Count > context.ContextSize)
                    {
                        var n_left = n_past - inferenceParams.TokensKeep;

                        n_past = Math.Max(1, inferenceParams.TokensKeep);

                        tokens.Clear();
                        tokens.AddRange(lastTokens.Skip(lastTokens.Count - n_left / 2).Take(n_left / 2));
                    }

                    n_past = await context.EvalAsync(tokens, n_past);
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


        /// <summary>
        /// Gets the handlers state.
        /// </summary>
        /// <returns></returns>
        public Task<InferenceHandlerState> GetStateAsync()
        {
            return Task.FromResult(default(InferenceHandlerState));
        }


        /// <summary>
        /// Sets the handlers state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Task SetStateAsync(InferenceHandlerState state)
        {
            return Task.CompletedTask;
        }
    }
}
