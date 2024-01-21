using Everest.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public interface IWebSocketRequestHandler
    {
        Task<bool> Handle(IHttpContext context, CancellationToken token);
    }
}