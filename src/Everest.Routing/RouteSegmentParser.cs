using System;
using System.Text.RegularExpressions;
using Everest.Collections;

namespace Everest.Routing
{
	public interface IRouteSegmentParser
	{
		bool TryParse(Iterator<string> segments, out ParameterCollection parameters);
	}

	public class AlphaNumericRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"^[a-zA-Z0-9-_\.]+$";
		
        public string Segment { get; }

		public AlphaNumericRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			Segment = segment;
		}
		
        public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null) 
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if(string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();
			return string.CompareOrdinal(Segment, segment) == 0;
		}
	}

	public class StringParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):string\}";

		public static Regex Regex { get; set; } = new Regex("^(.*)$", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public StringParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}

	public class IntParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):int\}";

		public static Regex Regex { get; set; } = new Regex("^-?[0-9]*$", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public IntParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}

	public class DoubleParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):double\}";

		public static Regex Regex { get; set; } = new Regex("^[+-]?([0-9]*[.])?[0-9]+", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public DoubleParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}

	public class FloatParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):float\}";

		public static Regex Regex { get; set; } = new Regex("^[+-]?([0-9]*[.])?[0-9]+", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public FloatParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}

	public class BoolParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):bool\}";

		public static Regex Regex { get; set; } = new Regex(@"^(?i:true|false)$", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public BoolParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}

	public class GuidParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):guid\}";

		public static Regex Regex { get; set; } = new Regex(@"^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public GuidParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}
	
	public class DateTimeParameterRouteSegmentParser : IRouteSegmentParser
	{
        public static string SegmentPattern = @"\{([^:]+):datetime\}";

		public static Regex Regex { get; set; } = new Regex(@"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?", RegexOptions.Compiled);

		public string Segment { get; }

		public string ParameterName { get; }

		public DateTimeParameterRouteSegmentParser(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			var match = Regex.Match(segment, SegmentPattern);
			if (!match.Success)
				throw new ArgumentException($"Invalid segment value: {segment}.");

			Segment = segment;
			ParameterName = match.Groups[1].Value;
		}

		public bool TryParse(Iterator<string> segments, out ParameterCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			var segment = segments.Current;
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException($"Segment required: {nameof(segment)}.");

			parameters = new ParameterCollection();

			if (!Regex.IsMatch(segment))
				return false;

			parameters[ParameterName] = segment;
			return true;
		}
	}
}
