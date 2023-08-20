using Everest.Services;
using System;

namespace Everest.Media
{
	public class MimeProviderConfigurator : ServiceConfigurator<MimeProvider>
	{
		public MimeProvider MimeProvider => Service;

		public MimeProviderConfigurator(MimeProvider mimeProvider, IServiceProvider services)
			: base(mimeProvider, services)
		{

		}

        public MimeProviderConfigurator AddMime(string fileExtension, string contentType, bool isBinary)
        {
            MimeProvider.AddMime(fileExtension, contentType, isBinary);
            return this;
        }
    }
}
