using Everest.Services;
using System;
using System.IO;
using System.IO.Compression;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<IResponseCompressor>
	{
		public IResponseCompressor Compressor => Service;

		public ResponseCompressorConfigurator(IResponseCompressor compressor, IServiceProvider services)
			: base(compressor, services)
		{
		}

		public ResponseCompressorConfigurator AddCompression(string encoding, Func<Stream, Stream> compression)
		{
			Compressor.AddCompression(encoding, compression);
			return this;
		}
	}

	public class DefaultResponseCompressorConfigurator : ResponseCompressorConfigurator
	{
		public new ResponseCompressor Compressor { get; }
		
		public DefaultResponseCompressorConfigurator(ResponseCompressor compressor, IServiceProvider services) 
			: base(compressor, services)
		{
			Compressor = compressor;
		}
	}

	public static class ResponseCompressorConfiguratorExtensions
	{
		public static ResponseCompressorConfigurator AddGzipCompression(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompression("gzip", input => new GZipStream(input, CompressionLevel.Fastest));
			return configurator;
		}

		public static ResponseCompressorConfigurator AddDeflateCompression(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompression("deflate", input => new DeflateStream(input, CompressionLevel.Fastest));
			return configurator;
		}

		public static ResponseCompressorConfigurator AddBrotliCompression(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompression("brotli", input => new BrotliStream(input, CompressionLevel.Fastest));
			return configurator;
		}
	}
}
