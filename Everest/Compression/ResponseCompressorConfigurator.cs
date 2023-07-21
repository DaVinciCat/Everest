using Everest.Services;
using System;
using System.IO;

namespace Everest.Compression
{
	public class ResponseCompressorConfigurator : ServiceConfigurator<IResponseCompressor>
	{
		public IResponseCompressor Compressor => Service;

		public ResponseCompressorConfigurator(ResponseCompressor compressor, IServiceProvider services)
			: base(compressor, services)
		{
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

		public ResponseCompressorConfigurator AddCompressor(string encoding, Func<Stream, Stream> compressor)
		{
			Compressor.Compressors.Add(encoding, compressor);
			return this;
		}
	}
}
