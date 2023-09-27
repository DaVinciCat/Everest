using System;
using Everest.Http;

namespace Everest.Compression
{
	public static class ResponseCompressionExtensions
	{
		public static bool SupportsContentCompression(this HttpRequest request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));

			return !string.IsNullOrWhiteSpace(request.Headers[HttpHeaders.AcceptEncoding]);
		}
	}
}
