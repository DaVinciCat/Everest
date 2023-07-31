﻿using Everest.Http;
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

		public async Task<bool> TryCompressResponseAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			
			Logger.LogTrace($"{context.Id} - Try to compress response: {new { Size = context.Response.Input.ToReadableSize() }}");

			if (context.Response.Input.Length < options.CompressionMinLength)
			{
				Logger.LogTrace($"{context.Id} - No response compression required: {new { Size = context.Response.Input.ToReadableSize(), CompressionMinLength = options.CompressionMinLength.ToReadableSize() }}");
				return false;
			}

			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers[AcceptEncodingHeader];
			if (acceptEncoding == null)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Missing header: {new { Header = AcceptEncodingHeader }}");
				return false;
			}

			var encodings = acceptEncoding.Split(',');
			if (encodings.Length == 0)
			{
				Logger.LogTrace($"{context.Id} - No response compression required. Empty header:  {new { Header = AcceptEncodingHeader }}");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Try to encode response: {new { AcceptEncodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.Select(compressor => compressor.Encoding).ToReadableArray() }}");

			foreach (var encoding in encodings)
			{
				if (сompressors.TryGetValue(encoding, out var compressor))
				{
					context.Response.Output = compressor.Compress(context.Response.OutputStream);

					context.Response.RemoveHeader(ContentEncodingHeader);
					context.Response.AddHeader(ContentEncodingHeader, encoding);

					return true;
				}
			}

			Logger.LogWarning($"{context.Id} - Failed to compress response. Header contains no supported encodings: {new { Header = AcceptEncodingHeader, Encodings = encodings.ToReadableArray(), SupportedEncodings = Compressors.Select(compressor => compressor.Encoding).ToReadableArray() }}");
			return false;
		}
	}
}
