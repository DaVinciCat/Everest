using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Everest.Mime
{
	public class MimeTypesProvider : IMimeTypesProvider
	{
		public MimeType[] MimeTypes => mimeTypes.Values.ToArray();
		
		private readonly Dictionary<string, MimeType> mimeTypes = new();

		public MimeTypesProvider()
		{
			var fields = typeof(MimeType).GetFields(BindingFlags.Public | BindingFlags.Static).ToArray();

			foreach (var field in fields)
			{
				if (field.GetValue(null) is not MimeType mimeType) 
					continue;

				mimeTypes[mimeType.FileExtension] = mimeType;
			}
		}

		public void AddMimeType(string fileExtension, string contentType, bool isBinary)
		{
			var mimeType = new MimeType(fileExtension, contentType, isBinary);
			mimeTypes[fileExtension] = mimeType;
		}

		public void RemoveMimeType(string fileExtension)
		{
			if(mimeTypes.ContainsKey(fileExtension))
			{
				mimeTypes.Remove(fileExtension);
			}
		}

		public void ClearMimeTypes()
		{
			mimeTypes.Clear();
		}

		public bool TryGetMimeType(string fileExtension, out MimeType mimeType)
		{
			return mimeTypes.TryGetValue(fileExtension, out mimeType);
		}
	}
}
