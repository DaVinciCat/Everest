﻿using Everest.Services;
using System;
using System.IO;
using System.IO.Compression;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<ResponseCompressorProvider>
	{
		public ResponseCompressorProvider ResponseCompressorProvider => Service;

		public ResponseCompressorConfigurator(ResponseCompressorProvider responseCompressorProvider, IServiceProvider services)
			: base(responseCompressorProvider, services)
		{
		}

		public ResponseCompressorConfigurator AddCompressor(string encoding, Func<Stream, Stream> compressor)
		{
			ResponseCompressorProvider.Compressors.Add(encoding, compressor);
			return this;
		}
	}
	
	public static class ResponseCompressorConfiguratorExtensions
	{
		public static ResponseCompressorConfigurator AddGzipCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("gzip", output => new GZipStream(output, CompressionLevel.Fastest));
			return configurator;
		}

		public static ResponseCompressorConfigurator AddDeflateCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("deflate", output => new DeflateStream(output, CompressionLevel.Fastest));
			return configurator;
		}

#if NET5_0_OR_GREATER
		public static ResponseCompressorConfigurator AddBrotliCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("brotli", output => new BrotliStream(output, CompressionLevel.Fastest));
			return configurator;
		}
#endif
    }
}
