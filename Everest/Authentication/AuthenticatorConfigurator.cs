using Everest.Services;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Authentication
{
	public class AuthenticatorConfigurator : ServiceConfigurator<IAuthenticator>
	{
		public IAuthenticator Authenticator => Service;

		public AuthenticatorConfigurator(IAuthenticator authenticator, IServiceProvider services)
			: base(authenticator, services)
		{

		}

		public void AddAuthentication(IAuthentication authentication)
		{
			Service.Authentications.Add(authentication);
		}
	}

	public class DefaultAuthenticatorConfigurator : AuthenticatorConfigurator
	{
		public new Authenticator Authenticator { get; }

		public DefaultAuthenticatorConfigurator(Authenticator authenticator, IServiceProvider services) 
			: base(authenticator, services)
		{
			Authenticator = authenticator;
		}
	}

	public static class AuthenticatorConfiguratorExtensions
	{
		public static AuthenticatorConfigurator AddBasicAuthentication(this AuthenticatorConfigurator configurator, Action<BasicAuthenticationOptions> options = null)
		{
			var configureOptions = new BasicAuthenticationOptions();
			options?.Invoke(configureOptions);

			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new BasicAuthentication(configureOptions, loggerFactory.CreateLogger<BasicAuthentication>());
			configurator.AddAuthentication(authentication);
			return configurator;
		}

		public static AuthenticatorConfigurator AddJwtTokenAuthentication(this AuthenticatorConfigurator configurator, Action<JwtAuthenticationOptions> options = null)
		{
			var configureOptions = new JwtAuthenticationOptions();
			options?.Invoke(configureOptions);

			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new JwtAuthentication(configureOptions, loggerFactory.CreateLogger<JwtAuthentication>());
			configurator.AddAuthentication(authentication);
			return configurator;
		}
	}
}
