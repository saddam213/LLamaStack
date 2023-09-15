using LLama;
using LLama.Abstractions;
using LLama.Native;

namespace LLamaStack.Core.Inference
{
    public static class InferenceExtensions
    {
        public static TokenData GetTokenData(this LLamaTokenDataArray tokenDataArray, LLamaContext context, int id)
        {
            // TODO: are all samplers sorted? if not we need to do a binary serach using id
            var tokenDataSpan = tokenDataArray.data[..1].Span;
            if (tokenDataSpan.Length == 0)
                throw new InvalidOperationException("The input sequence is empty.");

            var tokenData = tokenDataSpan[0];
            return new TokenData(tokenData.id)
            {
                Logit = tokenData.logit,
                Probability = tokenData.p,
                Content = context.TokenToString(tokenData.id)
            };
        }


        public static LLamaTokenDataArray ApplyPenalty(this LLamaContext context, IEnumerable<TokenData> lastTokens, IInferenceParams inferenceParams)
        {
            var repeatLastN = inferenceParams.RepeatLastTokensCount < 0
                ? context.ContextSize
                : inferenceParams.RepeatLastTokensCount;

            return context.ApplyPenalty
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



        public static int Sample(this LLamaContext context, LLamaTokenDataArray tokenDataArray, IInferenceParams inferenceParams, ref float? mirostatMu)
        {
            return context.Sample
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
                inferenceParams.Grammar
            );
        }


        private static IEnumerable<TokenData> TokenizeText(this LLamaContext context, string text, bool addBos)
        {
            return context.Tokenize(text, addBos).Select(x => new TokenData(x));
        }

        public static List<TokenData> TokenizeTextToList(this LLamaContext context, string text, bool addBos)
        {
            return context.TokenizeText(text, addBos).ToList();
        }

        public static TokenData[] TokenizeTextToArray(this LLamaContext context, string text, bool addBos)
        {
            return context.TokenizeText(text, addBos).ToArray();
        }


        public static Task<int> Eval(this LLamaContext context, IEnumerable<TokenData> tokens, int pastTokensCount)
        {
            return Task.Run(() => context.Eval(tokens.ToTokenIds(), pastTokensCount));
        }

        public static int[] ToTokenIds(this IEnumerable<TokenData> tokens)
        {
            return tokens.Select(x => x.Id).ToArray();
        }
    }
}
