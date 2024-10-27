using Everest.Core.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Authentication
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseAuthenticationMiddleware(this RestServerBuilder builder)
        {
            var authenticator = builder.Services.GetRequiredService<IAuthenticator>();
            builder.Middleware.Add(new AuthenticationMiddleware(authenticator));
            return builder;
        }
    }
}
