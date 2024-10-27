using System;
using System.Text;
using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class BasicAuthentication : IAuthentication, IHasLogger
	{
        ILogger IHasLogger.Logger => Logger;

        public ILogger<BasicAuthentication> Logger { get; }

		public string Scheme { get; set; } = "Basic";

		public Encoding Encoding { get; set; } = Encoding.UTF8;

		public string CredentialsDelimiter { get; set; } = ":";

		public BasicAuthentication(ILogger<BasicAuthentication> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public Task<bool> TryAuthenticateAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var header = context.Request.Headers[HttpHeaders.Authorization];
			if (string.IsNullOrWhiteSpace(header))
			{
            	Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to authenticate. Missing header: {new { Header = HttpHeaders.Authorization, Scheme = Scheme }}");
                return Task.FromResult(false);
			}

			if (header == Scheme)
			{
                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to authenticate. No credentials supplied: {new { Scheme = Scheme }}");
				return Task.FromResult(false);
			}

			if (!header.StartsWith(Scheme + ' ', StringComparison.OrdinalIgnoreCase))
			{
                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to authenticate. Incorrect header: {new { Header = HttpHeaders.Authorization, Scheme = Scheme }}");
                return Task.FromResult(false);
			}

			byte[] base64DecodedCredentials;
			try
			{
				var base64EncodedCredentials = header.Substring(Scheme.Length).Trim();
				base64DecodedCredentials = Convert.FromBase64String(base64EncodedCredentials);
			}
			catch (Exception ex)
			{
                Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to authenticate. Failed to convert credentials from Base64: {new { Scheme = Scheme }}"));
				return Task.FromResult(false);
			}

			string decodedCredentials;
			try
			{
				decodedCredentials = Encoding.GetString(base64DecodedCredentials);
			}
			catch (Exception ex)
			{
                Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to authenticate. Failed to decode Base64 credentials: {new { Encoding = Encoding.EncodingName, Scheme = Scheme }}"));
                return Task.FromResult(false);
			}

			var delimiterIndex = decodedCredentials.IndexOf(CredentialsDelimiter, StringComparison.OrdinalIgnoreCase);
			if (delimiterIndex == -1)
			{
                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to authenticate. Missing credentials delimiter: {new { Delimiter = CredentialsDelimiter, Scheme = Scheme }}");
                return Task.FromResult(false);
			}

			var username = decodedCredentials.Substring(0, delimiterIndex);
			var password = decodedCredentials.Substring(delimiterIndex + 1);
			var identity = new BasicIdentity(username, password);
			context.User.AddIdentity(identity);

            Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully authenticated: {new { Scheme = Scheme }}");
			return Task.FromResult(true);
		}
	}
}
