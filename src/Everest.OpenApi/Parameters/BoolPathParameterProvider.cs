using Microsoft.OpenApi.Models;
using System;
using System.Text.RegularExpressions;
using Everest.Routing;

namespace Everest.OpenApi.Parameters
{
    public class BoolPathParameterProvider : IOpenApiPathParameterProvider
    {
        public static string SegmentPattern => BoolParameterRouteSegmentParser.SegmentPattern;

        private readonly Func<Type, OpenApiSchema> getSchema;

        public BoolPathParameterProvider(Func<Type, OpenApiSchema> getSchema)
        {
            this.getSchema = getSchema;
        }

        public bool TryGetParameter(string segment, out OpenApiParameter parameter)
        {
            var match = Regex.Match(segment, SegmentPattern);
            if (match.Success)
            {
                parameter = new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = match.Groups[1].Value,
                    Required = true,
                    AllowEmptyValue = false,
                    Schema = getSchema(typeof(bool))
                };

                return true;
            }

            parameter = null;
            return false;
        }
    }
}
