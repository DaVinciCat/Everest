using System;
using System.Threading.Tasks;
using Everest.Core.Http;

namespace Everest.Core.Middlewares
{
    public class UseMiddleware : Middleware
    {
        private readonly Func<IHttpContext, Func<Task>, Task> middleware;

        public UseMiddleware(Func<IHttpContext, Func<Task>, Task> middleware)
        {
            this.middleware = middleware;
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
            await middleware(context, () => Next?.InvokeAsync(context));
        }
    }
}
