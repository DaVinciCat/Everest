using Everest.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Everest.Utils;

namespace Everest.Compression
{
	public class ResponseCompressionOptions
	{
		public int CompressionMinLength { get; set; } = 2048;
	}

	public class ResponseCompressor : IResponseCompressor
	{
		public string[] Encodings => Compressors.Keys.ToArray();

		public string AcceptEncodingHeader { get; set; } = "Accept-Encoding";

		public string ContentEncodingHeader { get; set; } = "Content-Encoding";

		public Dictionary<string, Func<Stream, Stream>> Compressors { get; set; } = new()
		{
			{ "gzip", stream => new GZipStream(stream, CompressionLevel.Fastest, true) },
			{ "deflate", stream => new DeflateStream(stream, CompressionLevel.Fastest, true) },
			{ "brotli", stream => new BrotliStream(stream, CompressionLevel.Fastest, true)}
		};

		private readonly ResponseCompressionOptions options;

		public ILogger<ResponseCompressor> Logger { get; }

		public ResponseCompressor(ILogger<ResponseCompressor> logger)
			: this(new ResponseCompressionOptions(), logger)
		{

		}

		public ResponseCompressor(ResponseCompressionOptions options, ILogger<ResponseCompressor> logger)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<bool> TryCompressResponseAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to compress response: {new { Size = context.Response.OutputStream.ToReadableSize() }}");

			if (context.Response.OutputStream.Length < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.Id} - No response compression required: {new { Size = context.Response.OutputStream.ToReadableSize(), CompressionMinLength = options.CompressionMinLength.ToReadableSize() }}");
				return false;
			}

			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers[AcceptEncodingHeader];
			if (acceptEncoding == null)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Missing header: {new { Header = AcceptEncodingHeader }}");
				return false;
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Empty header:  {new { Header = AcceptEncodingHeader }}");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Try to encode: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Encodings.ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (Compressors.TryGetValue(encoding, out var compressor))
				{
					var compressed = new MemoryStream();
					using (var stream = compressor(compressed))
					{
						context.Response.OutputStream.Position = 0;
						await context.Response.OutputStream.CopyToAsync(stream);
					}
					compressed.Position = 0;
					Logger.LogTrace($"{context.Id} - Successfully compressed response: {new { Size = context.Response.OutputStream.ToReadableSize(), CompressedSize = compressed.ToReadableSize(), Encoding = encoding }}");

					context.Response.OutputStream.SetLength(0);
					context.Response.OutputStream.Position = 0;
					await compressed.CopyToAsync(context.Response.OutputStream);
					context.Response.OutputStream.Position = 0;

					context.Response.RemoveHeader(ContentEncodingHeader);
					context.Response.AddHeader(ContentEncodingHeader, encoding);

					return true;
				}
			}

			Logger.LogWarning($"{context.Id} - Failed to compress response. Header contains no supported encodings: {new { Header = AcceptEncodingHeader, Encodings = encodings.ToReadableArray(), SupportedEncodings = Encodings.ToReadableArray() }}");
			return false;
		}
	}
}
