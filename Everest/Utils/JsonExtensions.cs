using System.Text.Json;

namespace Everest.Utils
{
	public static class JsonExtensions
	{
		public static string ToJson<T>(this T content)
		{
			return JsonSerializer.Serialize(content);
		}

		public static T FromJson<T>(this string json)
		{
			return JsonSerializer.Deserialize<T>(json);
		}
	}
}
