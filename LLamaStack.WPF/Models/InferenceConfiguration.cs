using LLamaStack.Core.Common;
using LLamaStack.Core.Config;
using LLamaStack.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace LLamaStack.WPF
{

    /// <summary>
    /// Inference configuration data
    /// </summary>
    /// <seealso cref="LLamaStack.Config.IInferenceConfiguration" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class InferenceConfiguration : IInferenceConfig, INotifyPropertyChanged
    {
        public int TokensKeep { get; set; } = 0;
        public int MaxTokens { get; set; } = -1;
        public int TopK { get; set; } = 40;
        public float TopP { get; set; } = 0.95f;
        public float TfsZ { get; set; } = 1.0f;
        public float TypicalP { get; set; } = 1.0f;
        public float Temperature { get; set; } = 0.8f;
        public float RepeatPenalty { get; set; } = 1.1f;
        public int RepeatLastTokensCount { get; set; } = 64;
        public float FrequencyPenalty { get; set; } = .0f;
        public float PresencePenalty { get; set; } = .0f;
        public SamplerType SamplerType { get; set; } = SamplerType.Default;
        public float MirostatTau { get; set; } = 5.0f;
        public float MirostatEta { get; set; } = 0.1f;
        public bool PenalizeNL { get; set; } = true;
        public List<LogitBiasModel> LogitBias { get; set; } = new List<LogitBiasModel>();


        public static InferenceConfiguration FromInferenceParams(IInferenceConfig inferenceConfig)
        {
            return new InferenceConfiguration
            {
                FrequencyPenalty = inferenceConfig.FrequencyPenalty,
                LogitBias = inferenceConfig.LogitBias,
                MaxTokens = inferenceConfig.MaxTokens,
                SamplerType = inferenceConfig.SamplerType,
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
                TypicalP = inferenceConfig.TypicalP
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
