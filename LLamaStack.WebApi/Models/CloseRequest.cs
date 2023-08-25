using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record CloseRequest([Required] Guid SessionId);
}
