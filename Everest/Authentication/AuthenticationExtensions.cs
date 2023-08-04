using System;
using Everest.Http;

namespace Everest.Authentication
{
	public static class AuthenticationExtensions
	{
		public static bool HasAuthentication(this HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			return !string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.Authorization]);
		}

		public static bool TryGetAuthenticationScheme(this HttpRequest request, out string scheme)
		{
			scheme = null;

			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if(string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.Authorization]))
				return false;

			scheme = request.Headers[HttpHeaders.Authorization].Split(' ')[0];
			return true;
		}

		public static bool SupportsAuthenticationScheme(this HttpRequest request, string scheme)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));

			return !string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.Authorization]) && request.Headers[HttpHeaders.Authorization].StartsWith(scheme + ' ', StringComparison.OrdinalIgnoreCase);
		}
	}
}
