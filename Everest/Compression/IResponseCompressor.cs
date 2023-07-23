using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressor
	{
		public ICompressor[] Compressors { get; }

		void AddCompressor(ICompressor compressor);

		void RemoveCompressor(ICompressor compressor);

		void RemoveCompressor(string encoding);

		void ClearCompressors();

		Task<bool> TryCompressResponseAsync(HttpContext context);
	}
}
