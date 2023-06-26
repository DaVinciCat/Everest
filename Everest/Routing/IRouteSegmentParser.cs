using System.Collections.Specialized;

namespace Everest.Routing
{
	public interface IRouteSegmentParser
	{
		bool TryParse(RouteSegment segment, string endPoint, out NameValueCollection parameters);
	}
}
