using System;
using System.Text;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Security;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class BasicAuthenticationOptions
	{
		public string Scheme { get; set; } = "Basic";

		public string Header { get; set; } = "Authorization";

		public string CredentialsDelimiter { get; set; } = ":";

		public Encoding Encoding { get; set; } = Encoding.UTF8;
	}

    public class BasicAuthentication : IAuthentication
    {
		public ILogger<BasicAuthentication> Logger { get; }

		public string Scheme => options.Scheme;

		private readonly BasicAuthenticationOptions options;
		
		public BasicAuthentication(BasicAuthenticationOptions options, ILogger<BasicAuthentication> logger)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public BasicAuthentication(ILogger<BasicAuthentication> logger)
			: this(new BasicAuthenticationOptions(), logger)
		{

		}

        public Task<bool> TryAuthenticateAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var header = context.Request.Headers[options.Header];
            if (string.IsNullOrEmpty(header))
            {
                Logger.LogWarning($"{context.Id} - Failed to authenticate. Missing header: {options.Header}. Scheme: {Scheme}. ");
                return Task.FromResult(false);
            }

            if (header == Scheme)
            {
                Logger.LogWarning($"{context.Id} - Failed to authenticate. No credentials supplied. Scheme: {Scheme}");
                return Task.FromResult(false);
            }

            if (!header.StartsWith(Scheme + ' ', StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogWarning($"{context.Id} - Failed to authenticate. Incorrect header: {options.Header}. Scheme: {Scheme}");
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
                Logger.LogError(ex, $"{context.Id} - Failed to authenticate. Failed to convert credentials from Base64. Scheme: {Scheme}");
                return Task.FromResult(false);
            }

            string decodedCredentials;
            try
            {
                decodedCredentials = options.Encoding.GetString(base64DecodedCredentials);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{context.Id} - Failed to authenticate. Failed to decode Base64 credentials to: {options.Encoding.EncodingName}. Scheme: {Scheme}");
                return Task.FromResult(false);
            }

            var delimiterIndex = decodedCredentials.IndexOf(options.CredentialsDelimiter, StringComparison.OrdinalIgnoreCase);
            if (delimiterIndex == -1)
            {
                Logger.LogWarning($"{context.Id} - Failed to authenticate. Missing credentials delimiter: {options.CredentialsDelimiter}. Scheme: {Scheme}");
                return Task.FromResult(false);
            }

            var username = decodedCredentials.Substring(0, delimiterIndex);
            var password = decodedCredentials.Substring(delimiterIndex + 1);
            var identity = new BasicIdentity(username, password);
            context.User.AddIdentity(identity);

            Logger.LogTrace($"{context.Id} - Successfully authenticated. Scheme: {Scheme}");
            return Task.FromResult(true);
        }
    }
}
