using System.IO;

namespace Everest.Compression
{
	public interface IStreamCompressor
	{
		string Encoding { get; }

		Stream Compress(Stream stream);
	}
}
