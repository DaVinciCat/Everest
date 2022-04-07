using System.Text.Json;

namespace Everest.Utils
{
	internal static class JsonExtensions
	{
		internal static string ToJson<T>(this T content)
		{
			return JsonSerializer.Serialize(content);
		}

		internal static T FromJson<T>(this string json)
		{
			return JsonSerializer.Deserialize<T>(json);
		}
	}
}
