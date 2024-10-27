using System;
using Everest.Core.Http;

namespace Everest.Compression
{
	public static class ResponseCompressionExtensions
	{
		public static bool SupportsContentCompression(this IHttpRequest request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));

			return !string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.AcceptEncoding]);
		}
	}
}
