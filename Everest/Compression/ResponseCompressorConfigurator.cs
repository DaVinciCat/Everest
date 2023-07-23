using Everest.Services;
using System;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<IResponseCompressor>
	{
		public IResponseCompressor Compressor => Service;

		public ResponseCompressorConfigurator(ResponseCompressor compressor, IServiceProvider services)
			: base(compressor, services)
		{
		}

		public ResponseCompressorConfigurator AddCompressor(ICompressor compressor)
		{
			Compressor.AddCompressor(compressor);
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
		public static ResponseCompressorConfigurator AddGzipCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor(new GZipCompressor());
			return configurator;
		}

		public static ResponseCompressorConfigurator AddDeflateCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor(new DeflateCompressor());
			return configurator;
		}

		public static ResponseCompressorConfigurator AddBrotliCompressor(this ResponseCompressorConfigurator configurator)
		{
			configurator.AddCompressor(new BrotliCompressor());
			return configurator;
		}
	}
}
