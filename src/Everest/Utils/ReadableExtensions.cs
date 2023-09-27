using System;
using System.Collections.Generic;
using System.IO;

namespace Everest.Utils
{
	public static class ReadableExtensions
	{
		public static string ToReadableSize(this Stream stream)
		{
			return ToReadableSize(stream?.Length ?? 0);
		}

		public static string ToReadableSize(this FileInfo file)
		{
			return ToReadableSize(file?.Length ?? 0);
		}

		public static string ToReadableSize(this byte[] bytes)
		{
			return ToReadableSize(bytes?.Length ?? 0);
		}

		public static string ToReadableSize(this int size)
		{
			return ToReadableSize(size, 0);
		}

		public static string ToReadableSize(this long size, int unit = 0)
		{
			string[] units = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

			while (size >= 1024)
			{
				size /= 1024;
				++unit;
			}

			return $"{size:G4} {units[unit]}";
		}

		public static string ToReadableArray(this IEnumerable<string> enumerable)
		{
			return string.Join(", ", enumerable ?? Array.Empty<string>() );
		}
	}
}
