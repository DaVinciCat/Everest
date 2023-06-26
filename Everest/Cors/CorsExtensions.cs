using System;
using Everest.Http;

namespace Everest.Cors
{
	public static class CorsHttpRequestExtensions
	{
		public static bool IsCorsPreflight(this HttpRequest request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));
			
			return request.HttpMethod == "OPTIONS" &&
			       request.Headers["Access-Control-Request-Method"] != null &&
			       request.Headers["Access-Control-Request-Headers"] != null &&
			       request.Headers["Origin"] != null;
		}
	}
}
