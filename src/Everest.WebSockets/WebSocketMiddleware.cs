using System.Threading;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;

namespace Everest.WebSockets
{
    public class WebSocketMiddleware<THandler> : Middleware
        where THandler : IWebSocketRequestHandler
    {
        private readonly THandler handler;

        public WebSocketMiddleware(THandler handler)
        {
            this.handler = handler;
        }
        
        public override async Task InvokeAsync(IHttpContext context)
        {
            if (await handler.Handle(context, CancellationToken.None))
            {
                return;
            }
            
            if(HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
