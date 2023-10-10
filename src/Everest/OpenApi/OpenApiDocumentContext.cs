using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public class OpenApiDocumentContext
    {
        public OpenApiDocument Document { get; }

        public OpenApiSchemaGenerator SchemaGenerator { get; }

        public OpenApiDocumentContext(OpenApiDocument document, OpenApiSchemaGenerator schemaGenerator)
        {
            Document = document;
            SchemaGenerator = schemaGenerator;
        }
    }
}
