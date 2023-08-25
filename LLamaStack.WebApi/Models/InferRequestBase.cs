using LLama.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    /// <summary>
    /// Using abstract class here instead of interface so we can share the attributes and defaults
    /// Virtual members so we can add/replace attributes etc in child classes
    /// </summary>
    public abstract class InferRequestBase
    {
        [Required]
        [DefaultValue("")]
        public virtual string Prompt { get; set; }

        [DefaultValue(0.0f)]
        public virtual float FrequencyPenalty { get; set; } = .0f;

        [DefaultValue(-1)]
        public virtual int MaxTokens { get; set; } = -1;

        [DefaultValue(MirostatType.Disable)]
        public virtual MirostatType Mirostat { get; set; } = MirostatType.Disable;

        [DefaultValue(0.1f)]
        public virtual float MirostatEta { get; set; } = 0.1f;

        [DefaultValue(5.0f)]
        [Range(0.1f, 10.0f)]
        public virtual float MirostatTau { get; set; } = 5.0f;

        [DefaultValue(true)]
        public virtual bool PenalizeNL { get; set; } = true;

        [DefaultValue(0.0f)]
        public virtual float PresencePenalty { get; set; } = .0f;

        [DefaultValue(64)]
        public virtual int RepeatLastTokensCount { get; set; } = 64;

        [DefaultValue(1.1f)]
        public virtual float RepeatPenalty { get; set; } = 1.1f;

        [DefaultValue(0.8f)]
        public virtual float Temperature { get; set; } = 0.8f;

        [DefaultValue(-1)]
        public virtual float TfsZ { get; set; } = 1.0f;

        [DefaultValue(0)]
        public virtual int TokensKeep { get; set; } = 0;

        [DefaultValue(40)]
        public virtual int TopK { get; set; } = 40;

        [DefaultValue(0.95f)]
        public virtual float TopP { get; set; } = 0.95f;

        [DefaultValue(1.0f)]
        public virtual float TypicalP { get; set; } = 1.0f;
    }
}