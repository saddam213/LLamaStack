using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record InferRequest : InferRequestBase
    {
        [Required]
        [DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
        public Guid SessionId { get; set; }

        [Required]
        [DefaultValue("What is an apple?")]
        public override string Prompt { get; set; }
    }
}
