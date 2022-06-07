using System;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public interface IRouteEndPointParser
	{
		bool TryParse(RouteSegment segment, string endPoint, NameValueCollection parameters = null);
	}

	public class RouteEndPointParser : IRouteEndPointParser
	{
		public char[] Delimiters { get; } = { '/', '#' };

		public bool TryParse(RouteSegment segment, string endPoint, NameValueCollection parameters = null)
		{
			parameters ??= new NameValueCollection();

			var segments = endPoint.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var iterator = new Iterator<string>(segments);

			return segment.TryParse(iterator, parameters) && !iterator.HasNext();
		}
	}
}

