using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Shell
{
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



