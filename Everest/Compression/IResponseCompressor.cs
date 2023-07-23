using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		public IStreamCompressor[] Compressors { get; }

		void AddCompressor(IStreamCompressor compressor);

		Task<bool> TryCompressResponseAsync(HttpContext context);
	}
}
