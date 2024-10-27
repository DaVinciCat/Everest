using System.Threading.Tasks;

namespace Everest.Core.WebSockets
{
    public interface IWebSocketContext
    {
        bool IsWebSocketRequest { get; }
        
        Task<WebSocketSession> AcceptWebSocketAsync();

        Task<WebSocketSession> AcceptWebSocketAsync(string subProtocol);
    }
}