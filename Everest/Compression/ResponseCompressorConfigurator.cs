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

		public ResponseCompressorConfigurator AddCompressor(IStreamCompressor compressor)
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
}
