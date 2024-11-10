using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public interface IWebSocketContext
    {
        bool IsWebSocketRequest { get; }
        
        Task<WebSocketSession> AcceptWebSocketAsync();

        Task<WebSocketSession> AcceptWebSocketAsync(string subProtocol);
    }
}