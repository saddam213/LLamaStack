using LLama.Abstractions;
using LLama.Common;
using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Helpers;
using System.Text;

namespace LLamaStack.Core.Extensions
{
    public static class Extensions
    {

        /// <summary>
        /// Converts an IModelConfig to IModelParams.
        /// </summary>
        /// <param name="modelConfig">The model configuration.</param>
        public static IModelParams ToModelParams(this IModelConfig modelConfig)
        {
            return new ModelParams(modelConfig.ModelPath)
            {
                BatchSize = modelConfig.BatchSize,
                ContextSize = modelConfig.ContextSize,
                ConvertEosToNewLine = modelConfig.ConvertEosToNewLine,
                EmbeddingMode = modelConfig.EmbeddingMode,
                Encoding = Encoding.GetEncoding(modelConfig.Encoding),
                GpuLayerCount = modelConfig.GpuLayerCount,
                LoraAdapter = modelConfig.LoraAdapter,
                LoraBase = modelConfig.LoraBase,
                LowVram = modelConfig.LowVram,
                MainGpu = modelConfig.MainGpu,
                ModelAlias = modelConfig.ModelAlias,
                MulMatQ = modelConfig.MulMatQ,
                Perplexity = modelConfig.Perplexity,
                RopeFrequencyBase = modelConfig.RopeFrequencyBase,
                RopeFrequencyScale = modelConfig.RopeFrequencyScale,
                Seed = modelConfig.Seed,
                TensorSplits = modelConfig.TensorSplits,
                UseFp16Memory = modelConfig.UseFp16Memory,
                UseMemoryLock = modelConfig.UseMemoryLock,
                UseMemorymap = modelConfig.UseMemorymap,
                Threads = modelConfig.Threads > 0
                    ? modelConfig.Threads
                    : Math.Max(Environment.ProcessorCount / 2, 1)
            };
        }

        /// <summary>
        /// Converts IInferenceConfig to InferenceParams.
        /// </summary>
        /// <param name="inferenceConfig">The inference configuration.</param>
        /// <returns>InferenceParams object</returns>
        public static InferenceParams ToInferenceParams(this IInferenceConfig inferenceConfig)
        {
            return new InferenceParams
            {
                FrequencyPenalty = inferenceConfig.FrequencyPenalty,
                MaxTokens = inferenceConfig.MaxTokens,
                Mirostat = inferenceConfig.SamplerType.ToMirostatType(),
                MirostatEta = inferenceConfig.MirostatEta,
                MirostatTau = inferenceConfig.MirostatTau,
                PenalizeNL = inferenceConfig.PenalizeNL,
                PresencePenalty = inferenceConfig.PresencePenalty,
                RepeatLastTokensCount = inferenceConfig.RepeatLastTokensCount,
                RepeatPenalty = inferenceConfig.RepeatPenalty,
                Temperature = inferenceConfig.Temperature,
                TfsZ = inferenceConfig.TfsZ,
                TokensKeep = inferenceConfig.TokensKeep,
                TopK = inferenceConfig.TopK,
                TopP = inferenceConfig.TopP,
                TypicalP = inferenceConfig.TypicalP,
                LogitBias = inferenceConfig.LogitBias?.ToDictionary(k => k.TokenId, v => v.Bias)
            };
        }


        /// <summary>
        /// Converts SamplerType to MirostatType.
        /// </summary>
        /// <param name="samplerType">Type of the sampler.</param>
        public static MirostatType ToMirostatType(this SamplerType samplerType)
        {
            return samplerType switch
            {
                SamplerType.Default => MirostatType.Disable,
                SamplerType.Mirostatv1 => MirostatType.Mirostat,
                SamplerType.Mirostatv2 => MirostatType.Mirostat2,
                _ => MirostatType.Disable
            };
        }


        /// <summary>
        /// Combines the AntiPrompts list and AntiPrompt csv 
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <returns>Combined AntiPrompts with duplicates removed</returns>
        public static List<string> GetAntiPrompts(this ISessionConfig sessionConfig)
        {
            return CombineCSV(sessionConfig.AntiPrompts, sessionConfig.AntiPrompt);
        }

        /// <summary>
        /// Combines the OutputFilters list and OutputFilter csv 
        /// </summary>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <returns>Combined OutputFilters with duplicates removed</returns>
        public static List<string> GetOutputFilters(this ISessionConfig sessionConfig)
        {
            return CombineCSV(sessionConfig.OutputFilters, sessionConfig.OutputFilter);
        }



        /// <summary>
        /// Combines a string list and a csv and removes duplicates
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="csv">The CSV.</param>
        /// <returns>Combined list with duplicates removed</returns>
        private static List<string> CombineCSV(List<string> list, string csv)
        {
            var results = list?.Count == 0
                ? StringHelpers.CommaSeperatedToList(csv)
                : StringHelpers.CommaSeperatedToList(csv).Concat(list);
            return results
                .Distinct()
                .ToList();
        }
    }
}
