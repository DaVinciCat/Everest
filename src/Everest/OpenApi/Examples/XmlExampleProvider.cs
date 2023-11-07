using System.IO;
using System.Xml.Serialization;

namespace Everest.OpenApi.Examples
{
    public abstract class XmlExampleProvider<T> : IOpenApiExampleProvider
    {
        string IOpenApiExampleProvider.GetExample()
        {
            var example = GetExample();

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, example);
                return writer.ToString();
            }
        }

        protected abstract T GetExample();
    }
}
