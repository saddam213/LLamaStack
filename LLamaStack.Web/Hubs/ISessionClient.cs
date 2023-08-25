using LLamaStack.Core.Models;
using LLamaStack.Web.Common;

namespace LLamaStack.Web.Hubs
{
    public interface ISessionClient
    {
        Task OnStatus(string connectionId, SessionConnectionStatus status);
        Task OnResponse(InferTokenModel token);
        Task OnError(string error);
    }
}
