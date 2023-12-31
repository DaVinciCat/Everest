﻿using System.IO;
using System.IO.Compression;

namespace Everest.ResponseCompression
{
	public static class CompressionExtensions
	{
		public static byte[] Gzip(this byte[] buffer, CompressionLevel level = CompressionLevel.Optimal)
		{
			using (var ms = new MemoryStream())
			using (var gzip = new GZipStream(ms, level))
			{
				gzip.Write(buffer, 0, buffer.Length);
				gzip.Close();

				return ms.ToArray();
			}
		}

		public static byte[] Deflate(this byte[] buffer, CompressionLevel level = CompressionLevel.Optimal)
		{
			using (var ms = new MemoryStream())
			using (var deflate = new DeflateStream(ms, level))
			{
				deflate.Write(buffer, 0, buffer.Length);
				deflate.Close();

				return ms.ToArray();
			}
		}
	}
}
