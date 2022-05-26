using System;
using System.Reflection;
using Everest.Annotations;
using Everest.Http;
using Everest.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Shell
{
	[RestResource("/api")]
	public static class Rest
	{
		[RestRoute("GET", "/welcome")]
		public static void Welcome(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			//context.Response.SendJson(new { Message = greetings, From = "Everest", Success = true });
		}

		[RestRoute("GET", "/welcome/{me}")]
		public static void WelcomeMe(HttpContext context)
		{
			var me = context.Request.PathParameters.GetParameterValue<string>("me");
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			context.Response.SendJson(new { Message = greetings, To = me, From = "Everest", Success = true });
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

			//DefaultLogger.LoggerFactory = loggerFactory;

			using (var rest = new RestServer())
			{
				//If you want to register routes manually
				rest.RegisterRoute("GET", "api/welcome", context =>
				{
					//context.Response.SendJson(new { Message = "Hello!", From = "Everest", Success = true });
				});

				rest.RegisterTransientService(() => new GreetingsService());
				//rest.ScanRoutes(Assembly.GetCallingAssembly());
				rest.Start("localhost", 8080);

				Console.WriteLine("GET localhost:8080/api/welcome");
				Console.WriteLine("GET localhost:8080/api/welcome/{me}");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();
			}
		}
	}

	#region Greetings

	public class GreetingsService
	{
		private readonly string[] greetings = new[]
		{
			"Hello!",
			"Hi!",
			"Welcome!",
			"Howdy!",
			"Hi, buddy!",
			"What's up?",
			"Salute!"
		};

		public string Greet()
		{
			var rnd = new Random(DateTime.Now.Millisecond);
			var index = rnd.Next(0, greetings.Length - 1);

			return greetings[index];
		}
	}

	public static class HttpContextExtensions
	{
		public static GreetingsService GetGreetingsService(this HttpContext context)
		{
			var service = context.Services.GetService<GreetingsService>();
			if (service == null)
				throw new NullReferenceException("GreetingsService required");

			return service;
		}
	}

	#endregion
}