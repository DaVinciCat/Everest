using Everest.Converters;
using Everest.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.IO.Compression;
using System.Linq;

namespace Everest.Compression
{
	public class ResponseCompressionOptions
	{
		public int CompressionMinLength { get; set; } = 2048;
	}

	public class ResponseCompressor : IResponseCompressor
	{
		public string[] Encodings => Compressors.Keys.ToArray();

		public Dictionary<string, Func<byte[], byte[]>> Compressors { get; set; } = new()
		{
			{ "gzip", bytes => bytes.Gzip(CompressionLevel.Fastest) },
			{ "deflate", bytes => bytes.Deflate(CompressionLevel.Fastest) }
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

		public bool TryCompressResponse(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			var content = context.Response.Body;

			Logger.LogTrace($"{context.Id} - Try compress response. Content to compress: [{content?.Length.ToReadableSize()}]");
			
			if (content == null || content.Length < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Content length: [{content?.ToReadableSize()}] < [{options.CompressionMinLength.ToReadableSize()}]");
				return false;
			}
			
			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers["Accept-Encoding"] ?? string.Empty;
			var encodings = acceptEncoding.Split(',');

			if (encodings.Length == 0)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Missing Accept-Encoding header");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Accept-Encoding: [{acceptEncoding}]. Supported encodings: [{string.Join(", ", Encodings)}]");

			foreach (var encoding in encodings)
			{
				if (Compressors.TryGetValue(encoding, out var compressor))
				{
					var compressed = compressor(content);
					context.Response.RemoveHeader("Content-Encoding");
					context.Response.AddHeader("Content-Encoding", encoding);
					context.Response.Write(compressed);

					Logger.LogTrace($"{context.Id} - Successfully compressed response: [{content.ToReadableSize()}] -> [{compressed.ToReadableSize()}]. Content-Encoding: {encoding}");
					return true;
				}
			}
			
			Logger.LogWarning($"{context.Id} - Failed to compress response. Accept-Encoding contains no supported encodings");
			return false;
		}
	}
}
