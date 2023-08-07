using System;
using Everest.Http;

namespace Everest.Cors
{
    public static class CorsHttpRequestExtensions
	{
		public static bool IsCorsPreflightRequest(this HttpRequest request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));
			
			return request.HttpMethod == HttpMethods.Options &&
			       request.Headers[HttpHeaders.AccessControlRequestMethod] != null &&
			       request.Headers[HttpHeaders.AccessControlRequestHeaders] != null &&
			       request.Headers[HttpHeaders.Origin] != null;
		}
	}
}
