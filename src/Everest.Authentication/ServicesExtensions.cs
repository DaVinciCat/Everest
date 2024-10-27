using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddAuthenticator(this IServiceCollection services, Func<IServiceProvider, IAuthenticator> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddAuthenticator(this IServiceCollection services, Action<AuthenticatorConfigurator> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            services.AddSingleton<IAuthenticator>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var authenticator = new Authenticator(loggerFactory.CreateLogger<Authenticator>());
                configurator(new AuthenticatorConfigurator(authenticator, provider));

                return authenticator;
            });

            return services;
        }
    }
}
