using System;
using Everest.Http;

namespace Everest.Authentication
{
	public static class AuthenticationExtensions
	{
		public static bool TryGetAuthenticationScheme(this IHttpRequest request, out string scheme)
		{
			scheme = null;

			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if(string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.Authorization]))
				return false;

			scheme = request.Headers[HttpHeaders.Authorization].Split(' ')[0];
			return true;
		}

		public static bool SupportsAuthentication(this IHttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			return !string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.Authorization]);
		}
	}
}
