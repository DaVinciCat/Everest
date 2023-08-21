using System;
using System.IO;
using Everest.Mime;

namespace Everest.Files
{
	public class StaticFileDescriptor
	{
		public FileInfo File { get; }

		public MimeType MimeType { get; }

		public StaticFileDescriptor(FileInfo file, MimeType mimeType)
		{
			File = file ?? throw new ArgumentNullException(nameof(file));
			MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
		}
	}
}
