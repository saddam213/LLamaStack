using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record GetRequest([Required] Guid SessionId);
}
