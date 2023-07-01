using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Everest.Compression
{
	public static class CompressionExtensions
	{
		public static async Task<Stream> GzipAsync(this Stream content, CompressionLevel level = CompressionLevel.Optimal)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			var ms = new MemoryStream();
			await using (var gzip = new GZipStream(ms, level, true))
			{
				await content.CopyToAsync(gzip);
			}

			ms.Position = 0;
			return ms;
		}

		public static async Task<Stream> DeflateAsync(this Stream content, CompressionLevel level = CompressionLevel.Optimal)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			var ms = new MemoryStream();
			await using (var deflate = new DeflateStream(ms, level, true))
			{
				await content.CopyToAsync(deflate);
			}

			ms.Position = 0;
			return ms;
		}
    }
}
