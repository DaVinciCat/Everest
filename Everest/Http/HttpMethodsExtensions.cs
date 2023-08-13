namespace Everest.Http
{
	public static class HttpMethodsExtensions
	{
		public static bool IsGetMethod(this HttpRequest request)
		{
			return request.HttpMethod == HttpMethods.Get;
		}

		public static bool IsHeadMethod(this HttpRequest request)
		{
			return request.HttpMethod == HttpMethods.Head;
		}
	}
}
