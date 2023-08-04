﻿using Everest.Http;
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
		public string[] Encodings => compressions.Keys.ToArray();
		
		private readonly Dictionary<string, Func<Stream, Stream>> compressions = new()
		{
			{ "gzip", input => new GZipStream(input, CompressionLevel.Fastest) },
			{ "deflate", input => new DeflateStream(input, CompressionLevel.Fastest) },
			{ "brotli", input => new BrotliStream(input, CompressionLevel.Fastest) }
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

		public void AddCompression(string encoding, Func<Stream, Stream> compression)
		{
			compressions[encoding] = compression ?? throw new ArgumentNullException(nameof(compression));
		}

		public void RemoveCompression(string encoding)
		{
			if (compressions.ContainsKey(encoding))
			{
				compressions.Remove(encoding);
			}
		}

		public void ClearCompressions()
		{
			compressions.Clear();
		}

		public Task<bool> TryComressResponseAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			
			if (context.Response.ContentLength < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No response compression required: {new { Length = context.Response.ContentLength.ToReadableSize(), CompressionMinLength = options.CompressionMinLength.ToReadableSize() }}");
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

			Logger.LogTrace($"{context.TraceIdentifier} - Try to create response compression stream: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = compressions.Keys.ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (compressions.TryGetValue(encoding, out var compression))
				{
					context.Response.WriteTo(to => compression(to));
					context.Response.RemoveHeader(HttpHeaders.ContentEncoding);
					context.Response.AddHeader(HttpHeaders.ContentEncoding, encoding);

					Logger.LogTrace($"{context.TraceIdentifier} - Successfully created response compression stream: {new { Encoding = encoding }}");
					return Task.FromResult(true);
				}
			}

			Logger.LogWarning($"{context.TraceIdentifier} - Failed to create response compression stream. Header contains no supported encodings: {new { Header = HttpHeaders.AcceptEncoding, Encodings = encodings.ToReadableArray(), SupportedEncodings = compressions.Keys.ToReadableArray() }}");
			return Task.FromResult(false);
		}
	}
}
