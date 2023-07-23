using System.IO;

namespace Everest.Compression
{
	public interface ICompressor
	{
		string Encoding { get; }

		Stream Compress(Stream stream);
	}
}
