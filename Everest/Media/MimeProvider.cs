using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Everest.Media
{
	public class MimeProvider : IMimeProvider
	{
		public Mime[] Mimes => mimes.Values.ToArray();
		
		private readonly Dictionary<string, Mime> mimes = new();

		public MimeProvider()
		{
			var fields = typeof(Mime).GetFields(BindingFlags.Public | BindingFlags.Static).ToArray();

			foreach (var field in fields)
			{
				if (field.GetValue(null) is not Mime mime) 
					continue;

				mimes[mime.FileExtension] = mime;
			}
		}

		public void AddMime(string fileExtension, string contentType, bool isBinary)
		{
			var mime = new Mime(fileExtension, contentType, isBinary);
			mimes[fileExtension] = mime;
		}

		public void RemoveMime(string fileExtension)
		{
			if(mimes.ContainsKey(fileExtension))
			{
				mimes.Remove(fileExtension);
			}
		}

		public void ClearMimes()
		{
			mimes.Clear();
		}

		public bool TryGetMime(string fileExtension, out Mime mime)
		{
			return mimes.TryGetValue(fileExtension, out mime);
		}
	}
}
