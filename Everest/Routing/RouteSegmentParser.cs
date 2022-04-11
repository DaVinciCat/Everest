using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public class RouteSegmentParser
	{
		public bool TryParse(RouteSegment segment, string url, NameValueCollection parameters = null)
		{
			parameters ??= new NameValueCollection();

			var segments = url.TrimStart('/').TrimEnd('/').Split('?')[0].Split("/");
			var iterator = new Iterator<string>(segments);

			return segment.TryParse(iterator, parameters) && !iterator.HasNext();
		}
	}
}

