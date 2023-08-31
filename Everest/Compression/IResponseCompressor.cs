using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		Task<bool> TryCompressResponseAsync(HttpContext context);
	}
}
