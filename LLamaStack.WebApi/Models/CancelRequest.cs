using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record CancelRequest([Required] Guid SessionId);

    public record GetRequest([Required] Guid SessionId);
    public record LoadRequest([Required] Guid SessionId);
    public record SaveRequest([Required] Guid SessionId, [Required] string Name);
}
