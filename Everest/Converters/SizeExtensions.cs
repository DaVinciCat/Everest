namespace Everest.Converters
{
	public static class SizeExtensions
	{
		public static string ToReadableSize(this byte[] bytes)
		{
			return ToReadableSize(bytes?.Length ?? 0);
		}

		public static string ToReadableSize(this int size, int unit = 0)
		{
			string[] units = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

			while (size >= 1024)
			{
				size /= 1024;
				++unit;
			}

			return $"{size:G4} {units[unit]}";
		}
	}
}
