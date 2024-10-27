namespace Everest.Core.Http
{
	public static class HttpMethodsExtensions
	{
		public static bool IsGetMethod(this IHttpRequest request)
		{
			return request.HttpMethod == HttpMethods.Get;
		}

		public static bool IsHeadMethod(this IHttpRequest request)
		{
			return request.HttpMethod == HttpMethods.Head;
		}
	}
}
