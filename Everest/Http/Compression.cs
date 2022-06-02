using Everest.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Everest.Http
{
	public interface ICompression
	{
		bool TryCompress(ref byte[] content, out string encoding);
	}

	public class Compression : ICompression
	{
		public int CompressionMinLength { get; set; } = 2048;

		public Dictionary<string, Func<byte[], byte[]>> Encoders = new()
		{
			{ "gzip", Gzip },
			{ "deflate", Deflate }
		};

		public string[] AcceptEncodings { get; }

		public Compression(string[] acceptEncodings)
		{
			AcceptEncodings = acceptEncodings;
		}

		public bool TryCompress(ref byte[] content, out string encoding)
		{
			if (content.Length < CompressionMinLength)
			{
				encoding = null;
				return false;
			}

			encoding = string.Empty;

			foreach (var enc in AcceptEncodings)
			{
				if (Encoders.TryGetValue(enc, out var action))
				{
					content = action(content);
					encoding = enc;
					return true;
				}
			}

			return false;
		}

		public static byte[] Gzip(byte[] content)
		{
			return content.Gzip(CompressionLevel.Fastest);
		}

		public static byte[] Deflate(byte[] content)
		{
			return content.Deflate(CompressionLevel.Fastest);
		}
	}
}
