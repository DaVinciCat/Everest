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
	public class ResponseCompressorProvider : IResponseCompressorProvider
	{
		public ILogger<ResponseCompressorProvider> Logger { get; }

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
			{ "gzip", output => new GZipStream(output, CompressionLevel.Fastest) },
			{ "deflate", output => new DeflateStream(output, CompressionLevel.Fastest) },
#if NET5_0_OR_GREATER
			{ "brotli", output => new BrotliStream(output, CompressionLevel.Fastest) },
#endif
        };

		public ResponseCompressorProvider(ILogger<ResponseCompressorProvider> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

        public Task<bool> TryGetResponseCompressorAsync(IHttpContext context, byte[] content, out string encoding, out Func<Stream, Stream> compressor)
        {
            compressor = null;
			encoding = null;

            if (content.Length < CompressionMinLength || !MediaTypes.Has(context.Response.ContentType))
            {
				return Task.FromResult(false);
            }

			return TryGetResponseCompressorAsync(context, out encoding, out compressor);
        }

        public Task<bool> TryGetResponseCompressorAsync(IHttpContext context, Stream stream, out string encoding, out Func<Stream, Stream> compressor)
        {
            compressor = null;
            encoding = null;

            if (stream.Length < CompressionMinLength || !MediaTypes.Has(context.Response.ContentType))
            {
                return Task.FromResult(false);
            }

            return TryGetResponseCompressorAsync(context, out encoding, out compressor);
        }

        private Task<bool> TryGetResponseCompressorAsync(IHttpContext context, out string encoding, out Func<Stream, Stream> compressor)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

            compressor = null;
            encoding = null;

            //TODO: super naive implementation, should replace it with q values support
            var header = context.Request.Headers[HttpHeaders.AcceptEncoding];
			if (header == null)
			{
				return Task.FromResult(false);
			}

			var encodings = header.Split(',');
			if (encodings.Length == 0)
			{
				return Task.FromResult(false);
			}

			if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace($"{context.TraceIdentifier} - Try to get response compressor: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

			foreach (var enc in encodings)
			{
				if (Compressors.TryGet(enc, out compressor))
                {
					encoding = enc;

                    if (Logger.IsEnabled(LogLevel.Trace))
                        Logger.LogTrace($"{context.TraceIdentifier} - Successfully got response compressor: {new { Encoding = encoding }}");

                    return Task.FromResult(true);
				}
			}

			if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning($"{context.TraceIdentifier} - Failed to get response compressor. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");
			
            return Task.FromResult(false);
		}
    }
}
