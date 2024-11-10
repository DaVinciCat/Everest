using System;
using System.IO;
using System.IO.Compression;
using Everest.Configuration;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<ResponseCompressor>
	{
		public ResponseCompressor ResponseCompressor => Service;

		public ResponseCompressorConfigurator(ResponseCompressor responseCompressor, IServiceProvider services)
			: base(responseCompressor, services)
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
