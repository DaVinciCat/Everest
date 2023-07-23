using System.IO.Compression;
using System.IO;

namespace Everest.Compression
{
	public class GZipCompressor : ICompressor
	{
		public string Encoding => "gzip";

		public Stream Compress(Stream stream)
		{
			return new GZipStream(stream, CompressionLevel.Fastest, true);
		}
	}

	public class DeflateCompressor : ICompressor
	{
		public string Encoding => "deflate";

		public Stream Compress(Stream stream)
		{
			return new DeflateStream(stream, CompressionLevel.Fastest, true);
		}
	}

	public class BrotliCompressor : ICompressor
	{
		public string Encoding => "brotli";

		public Stream Compress(Stream stream)
		{
			return new BrotliStream(stream, CompressionLevel.Fastest, true);
		}
	}
}
