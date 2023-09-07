using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record LoadRequest([Required] Guid SessionId);
}
