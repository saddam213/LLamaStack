using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record SaveRequest([Required] Guid SessionId, [Required] string Name);
}
