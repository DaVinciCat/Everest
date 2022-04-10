using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public class RouteSegmentMatcher
	{
		public bool Match(RouteSegment segment, string url, NameValueCollection parameters = null)
		{
			parameters ??= new NameValueCollection();

			var split = url.SplitUrl();
			var iterator = new Iterator<string>(split);

			return segment.Match(iterator, segment, parameters) && !iterator.HasNext();
		}

	}
}

