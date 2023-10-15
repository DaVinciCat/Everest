using System;
using Everest.Services;

namespace Everest.OpenApi
{
    public class OpenApiDocumentGeneratorConfigurator : ServiceConfigurator<OpenApiDocumentGenerator>
    {
        public OpenApiDocumentGenerator DocumentGenerator => Service;

        public OpenApiDocumentGeneratorConfigurator(OpenApiDocumentGenerator exceptionHandler, IServiceProvider services)
            : base(exceptionHandler, services)
        {

        }
    }
}
