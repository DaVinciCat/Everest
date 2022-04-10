using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Everest.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Shell
{
	class Program
	{
		static void Main()
		{
			//(/a*/FOO/:id)
			//{SPLAT}{FOO}{PARAM}
			//WEE/PEE/FOO/1

			//var template = "PATH";
			//var url = "/foo";
			//Segment segment = new PathSegment("foo");
			//var result = SegementProcessor.Process(segment, template, url);

			//template = "PATH/PARAM";
			//url = "/foo/1";
			//segment = new PathSegment("foo", new ParameterSegment("id"));
			//result = SegementProcessor.Process(segment, template, url);

			//template = "PATH/PARAM";
			//url = "/foo/1/wee";
			//segment = new PathSegment("foo", new ParameterSegment("id"));
			//result = SegementProcessor.Process(segment, template, url);

			//template = "SPLAT/PATH/PARAM";
			//url = "wee/boo/foo/1";
			//segment = new SplatSegment("a", new PathSegment("foo", new ParameterSegment("id")));
			//result = SegementProcessor.Process(segment, template, url);

			//var template = "SPLAT/PATH/PARAM/SPLAT";
			//var url = "wee/boo/foo/1/doo/dee";
			//var segment = new SplatSegment("a", new PathSegment("foo", new ParameterSegment("id", new SplatSegment("b", new ParameterSegment("guid")))));
			//var result = SegementProcessor.Process(segment, template, url);

			//var sb = new RouteSegmentBuilder();

			//sb.Builders.Add(@"\$", (name, next) => new StopRouteSegment(name, next));

			//var r1 = sb.Build("/foo");
			//var r2 = sb.Build("/foo/bar");
			//var r3 = sb.Build("/foo/bar/:id");
			//var r4 = sb.Build("/a*/foo/bar/b*");
			//var r5 = sb.Build("/foo/bar/:id/:guid");
			//var r6 = sb.Build("/foo/bar/$/:id/:guid");

			//var m = new RouteSegmentMatcher();
			//m.Matchers.Add(StopRouteSegment.Type, (iterator, _, _) =>
			//{
			//	while (iterator.MoveNext()) { }
			//	return true;
			//});

			//var b1 = m.Match(r1, "/foo");
			//var b2 = m.Match(r2, "/foo/bar");
			//var b3 = m.Match(r3, "/foo/bar/1");
			//var b4 = m.Match(r4, "/wee/gee/foo/bar/lee/zee");
			//var b5 = m.Match(r5, "/foo/bar/1/2");
			//var b6 = m.Match(r6, "/foo/bar/$/1/2");

			//var n1 = m.Match(r1, "/foo/bar/");
			//var n2 = m.Match(r2, "/foo/bar/:id");
			//var n3 = m.Match(r3, "/foo/bar/:id/wee");
			//var n4 = m.Match(r1, "/1");
			//var n5 = m.Match(r2, "/foo/1");

			//Console.ReadKey();

			var loggerFactory = LoggerFactory.Create(builder =>
			{
				builder.AddSimpleConsole(options =>
				{
					options.SingleLine = true;
					options.ColorBehavior = LoggerColorBehavior.Enabled;
					options.IncludeScopes = false;
					options.TimestampFormat = "hh:mm:ss:ffff ";
				});

				builder.SetMinimumLevel(LogLevel.Trace);
			});

			using (var rest = RestServerBuilder.Build())
			{
				rest.Routes.AddRoute("GET", "/home/:id", context =>
				{
					var id = context.Request.PathParameters.GetParameterValue<int>("id");
					context.Response.SendJson(new { Message = "Home Sweet Home", From = "Everest", Param = id, Success = true });
				});

				rest.Routes.AddRoute("GET", "/home", context =>
				{
					context.Response.SendJson(new { Message = "Home Sweet Home", From = "Everest", Success = true });
				});
				
				rest.Start("localhost", 8080);

				Console.WriteLine("GET localhost:8080/home");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();
			}
		}
	}
}



