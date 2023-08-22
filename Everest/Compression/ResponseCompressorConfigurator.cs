using Everest.Services;
using System;
using System.IO;
using System.IO.Compression;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<ResponseCompressor>
	{
		public ResponseCompressor ResponseCompressor => Service;

		public ResponseCompressorConfigurator(ResponseCompressor compressor, IServiceProvider services)
			: base(compressor, services)
		{
		}

		public ResponseCompressorConfigurator AddCompressor(string encoding, Func<Stream, Stream> compressor)
		{
			ResponseCompressor.Compressors.Add(encoding, compressor);
			return this;
		}
	}
	
	public static class ResponseCompressorConfiguratorExtensions
	{
		public static ResponseCompressorConfigurator AddGzipCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("gzip", input => new GZipStream(input, CompressionLevel.Fastest));
			return configurator;
		}

		public static ResponseCompressorConfigurator AddDeflateCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("deflate", input => new DeflateStream(input, CompressionLevel.Fastest));
			return configurator;
		}

		public static ResponseCompressorConfigurator AddBrotliCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor("brotli", input => new BrotliStream(input, CompressionLevel.Fastest));
			return configurator;
		}
	}
}
