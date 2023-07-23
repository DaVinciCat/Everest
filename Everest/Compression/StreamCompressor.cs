using System.IO.Compression;
using System.IO;

namespace Everest.Compression
{
	public class GZipStreamCompressor : IStreamCompressor
	{
		public string Encoding => "gzip";

		public Stream Compress(Stream stream)
		{
			return new GZipStream(stream, CompressionLevel.Fastest, true);
		}
	}

	public class DeflateStreamCompressor : IStreamCompressor
	{
		public string Encoding => "deflate";

		public Stream Compress(Stream stream)
		{
			return new DeflateStream(stream, CompressionLevel.Fastest, true);
		}
	}

	public class BrotliStreamCompressor : IStreamCompressor
	{
		public string Encoding => "brotli";

		public Stream Compress(Stream stream)
		{
			return new BrotliStream(stream, CompressionLevel.Fastest, true);
		}
	}
}
