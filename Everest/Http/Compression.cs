using Everest.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Everest.Http
{
	public class Compression
	{
		public int CompressionMinLength { get; set; } = 2048;

		private readonly Dictionary<string, Func<byte[], byte[]>> encodingActions = new Dictionary<string, Func<byte[], byte[]>>()
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
			encoding = string.Empty;

			foreach (var enc in AcceptEncodings)
			{
				if (encodingActions.TryGetValue(enc, out var action))
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
