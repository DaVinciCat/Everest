namespace Everest.Utils
{
	internal static class StringExtensions
	{
		internal static string SanitizeUrl(this string url)
		{
			return url.TrimStart('/').TrimEnd('/');
		}

		internal static string[] SplitUrl(this string url)
		{
			return SanitizeUrl(url).Split("/");
		}
	}
}
