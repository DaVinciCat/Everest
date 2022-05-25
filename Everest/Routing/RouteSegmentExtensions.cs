using System.Collections.Generic;

namespace Everest.Routing
{
	public static class RouteSegmentExtensions
	{
		public static string GetPath(this RouteSegment segment)
		{
			var segments = new List<string>();
			GetPathImpl(segment);
			return $"/{string.Join("/", segments)}";

			void GetPathImpl(RouteSegment next)
			{
				segments.Add(next.Value);
				if (next.HasNextSegment)
					GetPathImpl(next.NextSegment);
			}
		}
	}
}
