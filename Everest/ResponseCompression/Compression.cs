using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Everest.ResponseCompression
{
	public interface ICompression
	{
		bool TryCompress(byte[] content, out byte[] compressed, out string encoding);
	}

	public class Compression : ICompression
	{
		public int CompressionMinLength { get; set; } = 2048;
		
		public string[] AcceptEncodings { get; }

		public Dictionary<string, Func<byte[], byte[]>> Encoders = new()
		{
			{ "gzip", Gzip },
			{ "deflate", Deflate }
		};

		public Compression(string[] acceptEncodings)
		{
			AcceptEncodings = acceptEncodings;
		}

		public bool TryCompress(byte[] content, out byte[] compressed, out string encoding)
		{
			encoding = null;
			compressed = new byte[] { };

			if (content == null)
				return false;

			if (content.Length < CompressionMinLength)
				return false;
			
			foreach (var enc in AcceptEncodings)
			{
				if (Encoders.TryGetValue(enc, out var action))
				{
					compressed = action(content);
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
