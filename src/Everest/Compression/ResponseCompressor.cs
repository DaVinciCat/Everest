using Everest.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Everest.Mime;
using Everest.Utils;

namespace Everest.Compression
{
    public class ResponseCompressor : IResponseCompressor
	{
		public ILogger<ResponseCompressor> Logger { get; }

		public int CompressionMinLength { get; set; } = 2048;

		public ContentTypeCollection MediaTypes { get; } = new ContentTypeCollection
		{
			"text/plain",
			"text/css",
			"application/javascript",
			"text/javascript",
			"text/html",
			"application/xml",
			"text/xml",
			"application/json",
			"text/json",
			"application/wasm",
			"image/x-icon"
		};

		public CompressorCollection Compressors { get; } = new CompressorCollection
		{
			{ "gzip", input => new GZipStream(input, CompressionLevel.Fastest) },
			{ "deflate", input => new DeflateStream(input, CompressionLevel.Fastest) },
#if NET5_0_OR_GREATER
			{ "brotli", input => new BrotliStream(input, CompressionLevel.Fastest) },
#endif
        };

		public ResponseCompressor(ILogger<ResponseCompressor> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task<bool> TryCompressResponseAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (context.Response.ContentLength < CompressionMinLength)
			{
				return Task.FromResult(false);
			}

			if (!MediaTypes.Has(context.Response.ContentType))
			{
				return Task.FromResult(false);
			}

			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers[HttpHeaders.AcceptEncoding];
			if (acceptEncoding == null)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to compress response. Missing header: {new { Header = HttpHeaders.AcceptEncoding }}");
				return Task.FromResult(false);
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to compress response. Empty header: {new { Header = HttpHeaders.AcceptEncoding }}");
				return Task.FromResult(false);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Try to create response compression stream: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (Compressors.TryGet(encoding, out var compression))
				{
					context.Response.OutputStream = compression(context.Response.OutputStream);
					context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
					context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);

					Logger.LogTrace($"{context.TraceIdentifier} - Successfully created response compression stream: {new { Encoding = encoding }}");
					return Task.FromResult(true);
				}
			}

			Logger.LogWarning($"{context.TraceIdentifier} - Failed to create response compression stream. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");
			return Task.FromResult(false);
		}
	}
}
