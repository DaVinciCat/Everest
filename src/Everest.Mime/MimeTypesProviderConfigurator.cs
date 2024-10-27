using System;
using Everest.Common.Configuration;

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
            MimeTypesProvider.MimeTypes.Add(fileExtension, contentType, isBinary);
            return this;
        }
    }
}
