using System;
using Everest.Headers;
using Everest.Http;

namespace Everest.Cors
{
    public static class CorsHttpRequestExtensions
	{
		public static bool IsCorsPreflightRequest(this HttpRequest request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));
			
			return request.HttpMethod == "OPTIONS" &&
			       request.Headers[HeaderNames.AccessControlRequestMethod] != null &&
			       request.Headers[HeaderNames.AccessControlRequestHeaders] != null &&
			       request.Headers[HeaderNames.Origin] != null;
		}
	}
}
