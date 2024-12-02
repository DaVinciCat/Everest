using System;
using System.IO;
using Everest.Mime;

namespace Everest.StaticFiles
{
	public class StaticFileDescriptor
	{
		public FileInfo FileInfo { get; }

		public MimeType MimeType { get; }

		public StaticFileDescriptor(FileInfo fileInfo, MimeType mimeType)
		{
			FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
			MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
		}
	}
}
