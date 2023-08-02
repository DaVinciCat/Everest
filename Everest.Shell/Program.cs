using System;
using System.Reflection;
using System.Threading.Tasks;
using Everest.Authentication;
using Everest.Http;
using Everest.Rest;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Shell
{
	[RestResource("/api/1.0")]
	public static class Rest
	{
		[RestRoute("GET", "/welcome")]
		public static async Task Welcome(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			if (context.Request.QueryParameters.TryGetParameterValue("me", out string me))
			{
				await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = me, Success = true });
			}
			else
			{
				await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", Success = true });
			}
		}

		[RestRoute("GET", "/welcome/{me:string}")]
		public static async Task WelcomeMe(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();
			var me = context.Request.PathParameters.GetParameterValue<string>("me");

			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = me, Success = true });
		}
	}

	class Program
	{
		static void Main()
		{
			var services = new ServiceCollection();
			services.AddAuthenticator(configurator => configurator.AddBasicAuthentication())
					.AddCorsRequestHandler(configurator => configurator.AddDefaultCorsPolicy())
					.AddSingleton(_ => new GreetingsService())
					.AddConsoleLoggerFactory();

			using var rest = new RestServerBuilder(services)
				.UsePrefixes("http://localhost:8080/")
				.UseExceptionHandlerMiddleware()
				.UseRoutingMiddleware()
				.UseResponseCompressionMiddleware()
				.UseCorsMiddleware()
				.UseAuthenticationMiddleware()
				.UseEndPointMiddleware()
				.ScanRoutes(Assembly.GetExecutingAssembly())
				.Build();

			rest.Start();

			Console.WriteLine("GET localhost:8080/api/1.0/welcome");
			Console.WriteLine("GET localhost:8080/api/1.0/welcome/{me:string}");
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