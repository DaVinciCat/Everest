using Everest.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
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
		public ICompressor[] Compressors => сompressors.Values.ToArray();

		public string AcceptEncodingHeader { get; set; } = "Accept-Encoding";

		public string ContentEncodingHeader { get; set; } = "Content-Encoding";

		private readonly Dictionary<string, ICompressor> сompressors = new()
		{
			{ "gzip", new GZipCompressor() },
			{ "deflate", new DeflateCompressor() },
			{ "brotli", new BrotliCompressor() }
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

		public void AddCompressor(ICompressor compressor)
		{
			if (compressor == null) 
				throw new ArgumentNullException(nameof(compressor));

			сompressors[compressor.Encoding] = compressor;
		}

		public void RemoveCompressor(ICompressor compressor)
		{
			if (compressor == null) 
				throw new ArgumentNullException(nameof(compressor));

			if (сompressors.ContainsKey(compressor.Encoding))
			{
				сompressors.Remove(compressor.Encoding);
			}
		}

		public void RemoveCompressor(string encoding)
		{
			if (сompressors.ContainsKey(encoding))
			{
				сompressors.Remove(encoding);
			}
		}

		public void ClearCompressors()
		{
			сompressors.Clear();
		}

		public Task<bool> TryCompressResponseAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			
			if (context.Response.ContentLength < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No response compression required: {new { Length = context.Response.ContentLength.ToReadableSize(), CompressionMinLength = options.CompressionMinLength.ToReadableSize() }}");
				return Task.FromResult(false);
			}

			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers[AcceptEncodingHeader];
			if (acceptEncoding == null)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No response compression required. Missing header: {new { Header = AcceptEncodingHeader }}");
				return Task.FromResult(false);
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No response compression required. Empty header:  {new { Header = AcceptEncodingHeader }}");
				return Task.FromResult(false);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Try to match requested encoding: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.Select(compressor => compressor.Encoding).ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (сompressors.TryGetValue(encoding, out var compressor))
				{
					context.Response.PipeTo(to => compressor.Compress(to));

					context.Response.RemoveHeader(ContentEncodingHeader);
					context.Response.AddHeader(ContentEncodingHeader, encoding);

					Logger.LogTrace($"{context.TraceIdentifier} - Successfully matched request encoding:  {new { Encoding = encoding }}");
					return Task.FromResult(true);
				}
			}

			Logger.LogWarning($"{context.TraceIdentifier} - Failed to compress response. Header contains no supported encodings: {new { Header = AcceptEncodingHeader, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.Select(compressor => compressor.Encoding).ToReadableArray() }}");
			return Task.FromResult(false);
		}
	}
}
