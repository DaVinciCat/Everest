namespace Everest.Http
{
	public interface ICompressionProvider
	{
		ICompression GetCompression(HttpRequest request);
	}

	public class CompressionProvider : ICompressionProvider
	{
		public ICompression GetCompression(HttpRequest request)
		{
			//TODO: super naive implementation, should replace it with q values support
			var encodings = request.Headers["Accept-Encoding"] ?? string.Empty;
			var acceptEncodings = encodings.Split(',');

			return new Compression(acceptEncodings);
		}
	}
}
