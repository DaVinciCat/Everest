namespace Everest.Routing
{
	public class Route
	{
		public string Description => $"{HttpMethod} {Pattern}";

		public string HttpMethod { get; }

		public string Pattern { get; }

		public Route(string httpMethod, string pattern)
		{
			HttpMethod = httpMethod;
			Pattern = pattern;
		}
	}
}
