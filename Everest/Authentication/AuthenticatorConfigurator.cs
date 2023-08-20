using Everest.Services;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Authentication
{
	public class AuthenticatorConfigurator : ServiceConfigurator<Authenticator>
	{
		public Authenticator Authenticator => Service;

		public AuthenticatorConfigurator(Authenticator authenticator, IServiceProvider services)
			: base(authenticator, services)
		{

		}

		public void AddAuthentication(IAuthentication authentication)
		{
			Authenticator.AddAuthentication(authentication);
		}
	}
	
	public static class AuthenticatorConfiguratorExtensions
	{
		public static AuthenticatorConfigurator AddBasicAuthentication(this AuthenticatorConfigurator configurator, Action<BasicAuthentication> configure = null)
		{
			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new BasicAuthentication(loggerFactory.CreateLogger<BasicAuthentication>());
			configure?.Invoke(authentication);
			configurator.AddAuthentication(authentication);
			return configurator;
		}

		public static AuthenticatorConfigurator AddJwtTokenAuthentication(this AuthenticatorConfigurator configurator, Action<JwtAuthentication> configure = null)
		{
			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new JwtAuthentication(loggerFactory.CreateLogger<JwtAuthentication>());
			configure?.Invoke(authentication);
			configurator.AddAuthentication(authentication);
			return configurator;
		}
	}
}
