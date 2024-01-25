using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Mime;
using Everest.Utils;
using Microsoft.Extensions.Logging;

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

        public async Task<bool> TrySendCompressedResponseAsync(IHttpContext context, byte[] content, int offset, int count)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var length = count - offset;

            if (length < CompressionMinLength || !MediaTypes.Has(context.Response.ContentType))
            {
                return false;
            }

            //TODO: super naive implementation, should replace it with q values support
            var header = context.Request.Headers[HttpHeaders.AcceptEncoding];
            if (header == null)
            {
                return false;
            }

            var encodings = header.Split(',');
            if (encodings.Length == 0)
            {
                return false;
            }

            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace($"{context.TraceIdentifier} - Try to compress response: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

            foreach (var encoding in encodings)
            {
                if (Compressors.TryGet(encoding, out var compressor))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cp = compressor(ms))
                        {
                            try
                            {
                                context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
                                context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);

                                await cp.WriteAsync(content, offset, count);
                                cp.Close();

                                if (Logger.IsEnabled(LogLevel.Trace))
                                    Logger.LogTrace($"{context.TraceIdentifier} - Successfully compressed response: {new { Encoding = encoding, Length = length.ToReadableSize(), CompressedLength = ms.Length.ToReadableSize() }}");

                                await context.Response.SendResponseAsync(ms);
                            }
                            finally
                            {
                                ms.Close();
                            }
                        }
                    }
                    
                    return true;
                }
            }

            if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning($"{context.TraceIdentifier} - Failed to compress response. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

            return false;
        }

        public async Task<bool> TrySendCompressedResponseAsync(IHttpContext context, Stream stream)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var length = stream.Length;

            if (length < CompressionMinLength || !MediaTypes.Has(context.Response.ContentType))
            {
                return false;
            }

            //TODO: super naive implementation, should replace it with q values support
            var header = context.Request.Headers[HttpHeaders.AcceptEncoding];
            if (header == null)
            {
                return false;
            }

            var encodings = header.Split(',');
            if (encodings.Length == 0)
            {
                return false;
            }

            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace($"{context.TraceIdentifier} - Try to compress response: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

            foreach (var encoding in encodings)
            {
                if (Compressors.TryGet(encoding, out var compressor))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cp = compressor(ms))
                        {
                            try
                            {
                                context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
                                context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);

                                if (stream.CanSeek)
                                {
                                    stream.Position = 0;
                                }

                                var buffer = new byte[4096];
                                int read;

                                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await cp.WriteAsync(buffer, 0, read);
                                }
                                cp.Close();

                                if (Logger.IsEnabled(LogLevel.Trace))
                                    Logger.LogTrace($"{context.TraceIdentifier} - Successfully compressed response: {new { Encoding = encoding, Length = length.ToReadableSize(), CompressedLength = ms.Length.ToReadableSize() }}");

                                await context.Response.SendResponseAsync(ms);
                            }
                            finally
                            {
                                ms.Close();
                            }
                        }
                    }

                    return true;
                }
            }

            if (Logger.IsEnabled(LogLevel.Warning))
                Logger.LogWarning($"{context.TraceIdentifier} - Failed to compress response. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.ToReadableArray() }}");

            return false;
        }
    }
}
