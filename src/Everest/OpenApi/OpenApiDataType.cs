namespace Everest.OpenApi
{
    public class OpenApiDataType
    {
        public string Name { get; }

        public string Type { get; }

        public string Format { get; }

        public OpenApiDataType(string name, string type, string format)
        {
            Name = name;
            Type = type;
            Format = format;
        }

        public static OpenApiDataType Integer = new OpenApiDataType("integer", "integer", "int32");

        public static OpenApiDataType Long = new OpenApiDataType("long", "integer", "int64");

        public static OpenApiDataType Float = new OpenApiDataType("float", "number", "float");

        public static OpenApiDataType Double = new OpenApiDataType("double", "number", "double");

        public static OpenApiDataType String = new OpenApiDataType("string", "string", null);

        public static OpenApiDataType Byte = new OpenApiDataType("byte", "string", "byte");

        public static OpenApiDataType Binary = new OpenApiDataType("binary", "string", "binary");

        public static OpenApiDataType Boolean = new OpenApiDataType("boolean", "boolean", "binary");

        public static OpenApiDataType Date = new OpenApiDataType("date", "string", "date");

        public static OpenApiDataType DateTime = new OpenApiDataType("dateTime", "string", "date-time");

        public static OpenApiDataType Password = new OpenApiDataType("password", "string", "password");

        public static OpenApiDataType Uuid = new OpenApiDataType("uuid", "string", "uuid");

        public static OpenApiDataType Array = new OpenApiDataType("array", "array", null);

        public static OpenApiDataType Enum = new OpenApiDataType("enum", "enum", null);

        public static OpenApiDataType Object = new OpenApiDataType("object", "object", null);
    }
}
