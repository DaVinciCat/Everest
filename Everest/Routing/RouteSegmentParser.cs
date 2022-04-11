using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public class RouteSegmentParser
	{
		public bool TryParse(RouteSegment segment, string url, NameValueCollection parameters = null)
		{
			parameters ??= new NameValueCollection();

			var split = url.GetLeftPart().SplitUrl();
			var iterator = new Iterator<string>(split);

			return segment.TryParse(iterator, parameters) && !iterator.HasNext();
		}
	}
}

