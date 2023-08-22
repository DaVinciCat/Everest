using System.Linq;
using System.Reflection;

namespace Everest.Mime
{
	public class MimeTypesProvider : IMimeTypesProvider
    {
        public MimeTypeCollection MimeTypes { get; } = new();

        public MimeTypesProvider()
		{
			var fields = typeof(MimeType).GetFields(BindingFlags.Public | BindingFlags.Static).ToArray();

			foreach (var field in fields)
			{
				if (field.GetValue(null) is not MimeType mimeType) 
					continue;

                MimeTypes[mimeType.FileExtension] = mimeType;
			}
		}
		
		public bool TryGetMimeType(string fileExtension, out MimeType mimeType)
		{
			return MimeTypes.TryGet(fileExtension, out mimeType);
		}
	}
}
