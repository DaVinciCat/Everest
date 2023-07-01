using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
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

		public async Task<bool> TryParseAsync(RouteSegment segment, string endPoint, NameValueCollection parameters = null)
		{
			if (segment == null) 
				throw new ArgumentNullException(nameof(segment));
		
			if (endPoint == null) 
				throw new ArgumentNullException(nameof(endPoint));

			parameters ??= new NameValueCollection();

			var splitted = endPoint.Split(options.Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var segments = new Iterator<string>(splitted);

			return await segment.TryParseAsync(segments, parameters) && !segments.HasNext();
		}
	}
}

