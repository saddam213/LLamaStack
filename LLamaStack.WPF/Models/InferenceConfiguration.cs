using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace LLamaStack.WPF
{

    /// <summary>
    /// Inference configuration data
    /// </summary>
    /// <seealso cref="LLama.Abstractions.IInferenceParams" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class InferenceConfiguration : IInferenceParams, INotifyPropertyChanged
    {
        public int TokensKeep { get; set; } = 0;
        public int MaxTokens { get; set; } = -1;
        public string InputSuffix { get; set; } = string.Empty;
        public string InputPrefix { get; set; } = string.Empty;
        public int TopK { get; set; } = 40;
        public float TopP { get; set; } = 0.95f;
        public float TfsZ { get; set; } = 1.0f;
        public float TypicalP { get; set; } = 1.0f;
        public float Temperature { get; set; } = 0.8f;
        public float RepeatPenalty { get; set; } = 1.1f;
        public int RepeatLastTokensCount { get; set; } = 64;
        public float FrequencyPenalty { get; set; } = .0f;
        public float PresencePenalty { get; set; } = .0f;
        public MirostatType Mirostat { get; set; } = MirostatType.Disable;
        public float MirostatTau { get; set; } = 5.0f;
        public float MirostatEta { get; set; } = 0.1f;
        public bool PenalizeNL { get; set; } = true;
        public IEnumerable<string> AntiPrompts { get; set; } = Array.Empty<string>();
        public Dictionary<int, float> LogitBias { get; set; }
        public string PathSession { get; set; } = string.Empty;
        public SafeLLamaGrammarHandle Grammar { get; set; }

        public static InferenceConfiguration FromInferenceParams(IInferenceParams inferenceParams)
        {
            return new InferenceConfiguration
            {
                AntiPrompts = inferenceParams.AntiPrompts,
                FrequencyPenalty = inferenceParams.FrequencyPenalty,
                InputPrefix = inferenceParams.InputPrefix,
                InputSuffix = inferenceParams.InputSuffix,
                LogitBias = inferenceParams.LogitBias,
                MaxTokens = inferenceParams.MaxTokens,
                Mirostat = inferenceParams.Mirostat,
                MirostatEta = inferenceParams.MirostatEta,
                MirostatTau = inferenceParams.MirostatTau,
                PathSession = inferenceParams.PathSession,
                PenalizeNL = inferenceParams.PenalizeNL,
                PresencePenalty = inferenceParams.PresencePenalty,
                RepeatLastTokensCount = inferenceParams.RepeatLastTokensCount,
                RepeatPenalty = inferenceParams.RepeatPenalty,
                Temperature = inferenceParams.Temperature,
                TfsZ = inferenceParams.TfsZ,
                TokensKeep = inferenceParams.TokensKeep,
                TopK = inferenceParams.TopK,
                TopP = inferenceParams.TopP,
                TypicalP = inferenceParams.TypicalP
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
