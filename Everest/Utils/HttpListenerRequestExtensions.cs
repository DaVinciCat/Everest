using System.IO;
using System.Net;

namespace Everest.Utils
{
	internal static class HttpListenerRequestExtensions
	{
		internal static string GetEntityBody(this HttpListenerRequest request)
		{
			if (!request.HasEntityBody)
				return null;

			using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}

		internal static string GetAcceptEncoding(this HttpListenerRequest request)
		{
			if(request.Headers.TryGetValue<string>("Accept-Encoding", out var acceptEncoding))
			{
				return acceptEncoding;
			}

			return string.Empty;
		}

		internal static string[] GetAcceptEncodings(this HttpListenerRequest request)
		{
			//TODO: super naive implementation, should replace it with q values support
			return request.GetAcceptEncoding().Split(',');
		}
	}
}
