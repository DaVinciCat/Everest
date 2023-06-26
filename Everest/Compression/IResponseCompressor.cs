using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		bool TryCompressResponse(HttpContext context);
	}
}
