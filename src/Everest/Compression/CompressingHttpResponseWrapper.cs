using Everest.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
    public sealed class CompressingHttpResponseWrapper : HttpResponseWrapper
    {
        private readonly IResponseCompressorProvider provider;

        private readonly IHttpContext context;

        public CompressingHttpResponseWrapper(IHttpContext context, IResponseCompressorProvider provider)
            : base(context.Response)
        {
            this.context = context;
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override async Task SendResponseAsync(byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));


            if (!OutputStream.CanWrite)
            {
                throw new InvalidOperationException("Output stream is not writable");
            }

            try
            {
                Stream output; 

                if (await provider.TryGetResponseCompressorAsync(context, content, out var encoding, out var compressor))
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                        Logger.LogTrace($"{TraceIdentifier} - Sending compressed response: {new { RemoteEndPoint = RemoteEndPoint, ContentLength = content.ToReadableSize(), StatusCode = StatusCode, ContentType = ContentType, ContentEncoding = ContentEncoding?.EncodingName }}");

                    context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
                    context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);
                    output = compressor(context.Response.OutputStream);
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                        Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = RemoteEndPoint, ContentLength = content.Length.ToReadableSize(), StatusCode = StatusCode, ContentType = ContentType, ContentEncoding = ContentEncoding?.EncodingName }}");

                    ContentLength64 = content.Length;
                    output = OutputStream;
                }

                try
                {
                    await output.WriteAsync(content, 0, content.Length);
                }
                finally
                {
                    output.Close();
                }
            }
            catch
            {
                StatusCode = HttpStatusCode.InternalServerError;
                throw;
            }
            finally
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                    Logger.LogTrace($"{TraceIdentifier} - Response sent");

                CloseResponse();
            }
        }

        public override async Task SendResponseAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Input stream is not readable");
            }

            if (!OutputStream.CanWrite)
            {
                throw new InvalidOperationException("Output stream is not writable");
            }

            try
            {
                Stream output;

                if (await provider.TryGetResponseCompressorAsync(context, stream, out var encoding, out var compressor))
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                        Logger.LogTrace($"{TraceIdentifier} - Sending compressed response: {new { RemoteEndPoint = RemoteEndPoint, ContentLength = stream.Length.ToReadableSize(), StatusCode = StatusCode, ContentType = ContentType, ContentEncoding = ContentEncoding?.EncodingName }}");

                    context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
                    context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);
                    output = compressor(context.Response.OutputStream);
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                        Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = RemoteEndPoint, ContentLength = stream.Length.ToReadableSize(), StatusCode = StatusCode, ContentType = ContentType, ContentEncoding = ContentEncoding?.EncodingName }}");

                    ContentLength64 = stream.Length;
                    output = OutputStream;
                }

                try
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }

                    var buffer = new byte[4096];
                    int read;

                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, read);
                    }
                }
                finally
                {
                    output.Close();
                }
            }
            catch
            {
                StatusCode = HttpStatusCode.InternalServerError;
                throw;
            }
            finally
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                    Logger.LogTrace($"{TraceIdentifier} - Response sent");

                CloseResponse();
            }
        }
    }
}
