using System;
using Everest.Services;

namespace Everest.Mime
{
	public class MimeTypesProviderConfigurator : ServiceConfigurator<MimeTypesProvider>
	{
		public MimeTypesProvider MimeTypesProvider => Service;

		public MimeTypesProviderConfigurator(MimeTypesProvider mimeTypesProvider, IServiceProvider services)
			: base(mimeTypesProvider, services)
		{

		}

        public MimeTypesProviderConfigurator AddMimeType(string fileExtension, string contentType, bool isBinary)
        {
            MimeTypesProvider.AddMimeType(fileExtension, contentType, isBinary);
            return this;
        }
    }
}
