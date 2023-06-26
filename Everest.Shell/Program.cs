using System;
using System.Reflection;
using Everest.Http;
using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Shell
{
    [RestResource("api/1.0")]
	public static class Rest
	{
		[RestRoute("GET", "welcome")]
		public static void Welcome(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			context.Response.WriteJson(new { Message = greetings, From = "Everest", Success = true });
		}

		[HttpGet("welcome/{me:string}")]
		public static void WelcomeMe(HttpContext context)
		{
			var me = context.Request.PathParameters.GetParameterValue<string>("me");
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			context.Response.WriteJson(new { Message = greetings, To = me, From = "Everest", Success = true });
		}
	}

	class Program
	{
		static void Main()
		{
			var services = new ServiceCollection();
			services.AddDefaults()
					.AddSingleton(_ => new GreetingsService())
					.AddConsoleLoggerFactory();

			using var rest = new RestServerBuilder(services)
				.UsePrefixes("http://localhost:8080/")
				.UseExceptionHandlerMiddleware()
				.UseCorsMiddleware()
				.UseRoutingMiddleware()
				.UseAuthenticationMiddleware(b => b.AddBasicAuthentication())
				.UseEndPointMiddleware()
				.UseResponseCompressionMiddleware()
				.UseResponseMiddleware()
				.ScanRoutes(Assembly.GetExecutingAssembly())
				.Build();

			rest.Start();

			Console.WriteLine("GET localhost:8080/api/1.0/welcome");
			Console.WriteLine("GET localhost:8080/api/1.0/welcome/{me}");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
	}

	#region Greetings

	public class GreetingsService
	{
		public GreetingsService()
		{
			//for test purposes only
		}

		private readonly string[] greetings = 
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
			return context.Services.GetRequiredService<GreetingsService>();
		}
	}

	#endregion
}