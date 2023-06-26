using System;
using System.Collections.Specialized;
using Everest.Collections;

namespace Everest.Routing
{
	public class RouteSegmentParserOptions
	{
		public char[] Delimiters { get; set; } = { '/' };
	}

	public class RouteSegmentParser : IRouteSegmentParser
	{
		private readonly RouteSegmentParserOptions options;

		public RouteSegmentParser()
			: this(new RouteSegmentParserOptions())
		{
			
		}

		public RouteSegmentParser(RouteSegmentParserOptions options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public bool TryParse(RouteSegment segment, string endPoint, out NameValueCollection parameters)
		{
			if (segment == null) 
				throw new ArgumentNullException(nameof(segment));
		
			if (endPoint == null) 
				throw new ArgumentNullException(nameof(endPoint));

			parameters = new NameValueCollection();

			var splitted = endPoint.Split(options.Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var segments = new Iterator<string>(splitted);

			return segment.TryParse(segments, parameters) && !segments.HasNext();
		}
	}
}

