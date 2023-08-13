using System;
using System.IO;
using Everest.Media;

namespace Everest.Files
{
	public class StaticFileDescriptor
	{
		public FileInfo File { get; }

		public MimeDescriptor Mime { get; }

		public StaticFileDescriptor(FileInfo file, MimeDescriptor mime)
		{
			File = file ?? throw new ArgumentNullException(nameof(file));
			Mime = mime ?? throw new ArgumentNullException(nameof(mime));
		}
	}
}
