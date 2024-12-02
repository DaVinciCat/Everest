using System;
using Everest.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class AuthenticatorConfigurator : ServiceConfigurator<Authenticator>
	{
		public Authenticator Authenticator => Service;

		public AuthenticatorConfigurator(Authenticator authenticator, IServiceProvider services)
			: base(authenticator, services)
		{

		}

		public AuthenticatorConfigurator AddAuthentication(string scheme, IAuthentication authentication)
		{
			Authenticator.Authentications.Add(scheme, authentication);
			return this;
		}
	}
	
	public static class AuthenticatorConfiguratorExtensions
	{
		public static AuthenticatorConfigurator AddBasicAuthentication(this AuthenticatorConfigurator configurator, Action<BasicAuthentication> configure = null)
		{
			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new BasicAuthentication(loggerFactory.CreateLogger<BasicAuthentication>());
			configure?.Invoke(authentication);
			configurator.AddAuthentication(authentication.Scheme, authentication);
			return configurator;
		}

		public static AuthenticatorConfigurator AddJwtTokenAuthentication(this AuthenticatorConfigurator configurator, Action<JwtAuthentication> configure = null)
		{
			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new JwtAuthentication(loggerFactory.CreateLogger<JwtAuthentication>());
			configure?.Invoke(authentication);
			configurator.AddAuthentication(authentication.Scheme, authentication);
			return configurator;
		}
	}
}
