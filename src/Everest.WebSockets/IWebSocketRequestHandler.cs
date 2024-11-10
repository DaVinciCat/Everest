using System.Threading;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.WebSockets
{
    public interface IWebSocketRequestHandler
    {
        Task<bool> Handle(IHttpContext context, CancellationToken token);
    }
}