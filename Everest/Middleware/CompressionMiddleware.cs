using Everest.Http;
using Everest.Utils;

namespace Everest.Middleware
{
	public class CompressionMiddleware : MiddlewareBase
	{
		private readonly ICompressionProvider provider;

		public CompressionMiddleware(ICompressionProvider provider)
		{
			this.provider = provider;
		}

		public override void Invoke(HttpContext context)
		{
			var compression = provider.GetCompression(context.Request);

			if (compression.TryCompress(context.Response.Body, out var compressed, out var encoding))
			{
				context.Response.RemoveHeader("Content-Encoding");
				context.Response.AddHeader("Content-Encoding", encoding);
				context.Response.Write(compressed);
			}

			if (HasNext)
				Next.Invoke(context);
		}
	}
}
