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
				return Task.FromResult(false);
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				return Task.FromResult(false);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Try to set compressing response writer: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (Compressors.TryGet(encoding, out var compression))
                {
                    context.Response.ResponseWriter = new CompressingResponseWriter(context.Response, MediaTypes, compression, encoding, CompressionMinLength);

                    Logger.LogTrace($"{context.TraceIdentifier} - Successfully set compressing response writer: {new { Encoding = encoding }}");
                    return Task.FromResult(true);
				}
			}

			Logger.LogWarning($"{context.TraceIdentifier} - Failed to set compressing response writer. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");
			return Task.FromResult(false);
		}

		private class CompressingResponseWriter : HttpResponseWriter
        {
            private readonly HttpResponse response;

            private readonly ContentTypeCollection mediaTypes;

            private readonly Func<Stream, Stream> compression;

            private readonly string encoding;
            
            private readonly int compressionMinLength;

            public CompressingResponseWriter(HttpResponse response, ContentTypeCollection mediaTypes, Func<Stream, Stream> compression, string encoding, int compressionMinLength)
                : base(response)
            {
                this.response = response;
                this.mediaTypes = mediaTypes;
                this.compression = compression;
                this.encoding = encoding;
                this.compressionMinLength = compressionMinLength;
            }

            public override async Task Write(byte[] content)
            {
                Stream output;
                var shouldClose = false;
                if (content.Length >= compressionMinLength && mediaTypes.Has(response.ContentType))
                {
                    output = compression(response.OutputStream);
                    shouldClose = true;

                    response.RemoveHeader(HttpHeaders.ContentEncoding);
                    response.AddHeader(HttpHeaders.ContentEncoding, encoding);

                    response.Logger.LogTrace($"{response.TraceIdentifier} - Response successfully compressed: {new { Encoding = encoding }}");
                }
                else
                {
                    output = response.OutputStream;
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
            }

            public override async Task Write(Stream stream)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                Stream output;
                var shouldClose = false;
                if (stream.Length >= compressionMinLength && mediaTypes.Has(response.ContentType))
                {
                    output = compression(response.OutputStream);
                    shouldClose = true;

                    response.RemoveHeader(HttpHeaders.ContentEncoding);
                    response.AddHeader(HttpHeaders.ContentEncoding, encoding);
                }
                else
                {
                    output = response.OutputStream;
                }

                try
                {
                    var buffer = new byte[4096];
                    int read;

                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, read);
                    }

                    response.Logger.LogTrace($"{response.TraceIdentifier} - Response successfully compressed: {new { Encoding = encoding }}");
                }
                finally
                {
                    if (shouldClose)
                    {
                        output.Close();
                    }
                }
            }
        }
    }
}
