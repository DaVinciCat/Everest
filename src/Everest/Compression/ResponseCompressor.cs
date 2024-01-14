using Everest.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Everest.Mime;
using Everest.Utils;
using System.IO;

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
			{ "gzip", output => new GZipStream(output, CompressionLevel.Fastest, true) },
			{ "deflate", output => new DeflateStream(output, CompressionLevel.Fastest, true) },
#if NET5_0_OR_GREATER
			{ "brotli", output => new BrotliStream(output, CompressionLevel.Fastest, true) },
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
					context.Response.ResponseContentWriter = async (response, content) =>
					{
						Stream output;
						var shouldClose = false;
						if (content.Length >= CompressionMinLength && MediaTypes.Has(context.Response.ContentType))
						{
							output = compression(response.OutputStream);
							shouldClose = true;

							context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
							context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);
						}
						else
						{
							output = context.Response.OutputStream;
						}

						try
						{
							await output.WriteAsync(content, 0, content.Length);
						}
						finally
						{
							if (shouldClose)
							{
								output.Close();
							}
						}
					};

					context.Response.ResponseStreamWriter = async (response, stream) =>
					{
						if (stream.CanSeek)
						{
							stream.Position = 0;
						}

						Stream output;
						var shouldClose = false;
						if (stream.Length >= CompressionMinLength && MediaTypes.Has(context.Response.ContentType))
						{
							output = compression(response.OutputStream);
							shouldClose = true;

							context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
							context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);
						}
						else
						{
							output = context.Response.OutputStream;
						}

						try
						{
							var buffer = new byte[4096];
							int read;

							while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
							{
								await output.WriteAsync(buffer, 0, read);
							}
						}
						finally
						{
							if (shouldClose)
							{
								output.Close();
							}
						}
					};

					Logger.LogTrace($"{context.TraceIdentifier} - Successfully created response compression stream: {new { Encoding = encoding }}");
					return Task.FromResult(true);
				}
			}

			Logger.LogWarning($"{context.TraceIdentifier} - Failed to create response compression stream. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");
			return Task.FromResult(false);
		}
	}
}
