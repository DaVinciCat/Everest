using System;
using System.Reflection;
using Everest.Http;
using Everest.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Shell
{
	[RestResource("/api")]
	public static class Rest
	{
		[RestRoute("GET", "/home")]
		public static void Home(HttpContext context)
		{
			context.Response.SendJson(new { Message = "Home Sweet Home", From = "Everest", Success = true });
		}
	}

	class Program
	{
		static void Main()
		{
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

			using (var rest = RestServerBuilder.Build(loggerFactory))
			{
				rest.RegisterRoute("GET", "/home", context =>
				{
					context.Response.SendJson(new { Message = "Home Sweet Home", From = "Everest", Success = true });
				}); 

				rest.ScanRoutes(Assembly.GetCallingAssembly());
				rest.Start("localhost", 8080);

				Console.WriteLine("GET localhost:8080/home");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();
			}
		}
	}
}