using System.Threading;
using System.Threading.Tasks;
using Everest.Core.Http;

namespace Everest.WebSockets
{
    public interface IWebSocketRequestHandler
    {
        Task<bool> Handle(IHttpContext context, CancellationToken token);
    }
}