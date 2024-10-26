using System;
using Everest.Services;

namespace Everest.OpenApi
{
    public class OpenApiDocumentGeneratorConfigurator : ServiceConfigurator<OpenApiDocumentGenerator>
    {
        public OpenApiDocumentGenerator DocumentGenerator => Service;

        public OpenApiDocumentGeneratorConfigurator(OpenApiDocumentGenerator generator, IServiceProvider services)
            : base(generator, services)
        {

        }
    }
}
