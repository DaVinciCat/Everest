using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Everest.Routing
{
	public interface IRouteSegmentParser
	{
		Task<bool> TryParseAsync(RouteSegment segment, string endPoint, NameValueCollection parameters = null);
	}
}
