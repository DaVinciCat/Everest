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
			
			Logger.LogTrace($"{context.Id} - Try to compress response. Response size: [{context.Response.OutputStream.ToReadableSize()}]");
			
			if (context.Response.OutputStream == null || context.Response.OutputStream.Length < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.Id} - No response compression required: [{context.Response.OutputStream.ToReadableSize()}] < [{options.CompressionMinLength.ToReadableSize()}]");
				return false;
			}
			
			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers["Accept-Encoding"];
			if (acceptEncoding == null)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Accept-Encoding header is missing");
				return false;
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Accept-Encoding header contains no encodings");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Accept-Encoding: [{acceptEncoding}]. Supported encodings: [{string.Join(", ", Encodings)}]");

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
					Logger.LogTrace($"{context.Id} - Successfully compressed response: [{context.Response.OutputStream.ToReadableSize()}] -> [{compressed.ToReadableSize()}]. Content-Encoding: {encoding}");

					context.Response.OutputStream.SetLength(0);
					context.Response.OutputStream.Position = 0;
					await compressed.CopyToAsync(context.Response.OutputStream);
					context.Response.OutputStream.Position = 0;

					context.Response.RemoveHeader("Content-Encoding");
					context.Response.AddHeader("Content-Encoding", encoding);
					
					return true;
				}
			}
			
			Logger.LogWarning($"{context.Id} - Failed to compress response. Accept-Encoding contains no supported encodings");
			return false;
		}
	}
}
