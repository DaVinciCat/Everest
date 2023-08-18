using Everest.Services;
using System;

namespace Everest.Media
{
	public class MimeProviderConfigurator : ServiceConfigurator<IMimeProvider>
	{
		public IMimeProvider MimeProvider => Service;

		public MimeProviderConfigurator(IMimeProvider mimeProvider, IServiceProvider services)
			: base(mimeProvider, services)
		{

		}
	}

	public class DefaultMimeProviderConfigurator : MimeProviderConfigurator
	{
		public new MimeProvider MimeProvider { get; }

		public DefaultMimeProviderConfigurator(MimeProvider mimeProvider, IServiceProvider services)
			: base(mimeProvider, services)
		{
			MimeProvider = mimeProvider;
		}

		public DefaultMimeProviderConfigurator AddMime(string fileExtension, string contentType, bool isBinary)
		{
			MimeProvider.AddMime(fileExtension, contentType, isBinary);
			return this;
		}
	}
}
