using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Everest.Authentication;
using Everest.Http;
using Everest.Rest;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Shell
{
	[RestResource("/api/1.0/tests")]
	public static class Rest
	{
		[HttpGet("/get/static-route-no-parameters")]
		public static async Task GetStaticRouteNoParameters(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();
			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", Success = true });
		}


		[HttpGet("/get/static-route-text")]
		public static async Task GetStaticRouteText(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();
			
			await context.Response.WriteTextAsync("lorem\n");
			await context.Response.WriteTextAsync("ipsum\n");
			await context.Response.WriteTextAsync("dolor\n");
		}

		[HttpGet("/get/static-route-with-query-parameter-me")]
		public static async Task GetStaticRouteWithQueryParamaterMe(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			var me = context.Request.QueryParameters.GetParameterValue<string>("me");
			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = me, Success = true });
		}

		[HttpGet("/get/route-with-path-parameter-me/{me:string}")]
		public static async Task GetRouteWithPathParameterMe(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();
			var me = context.Request.PathParameters.GetParameterValue<string>("me");

			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = me, Success = true });
		}
		
		[HttpGet("/get/route-with-path-parameter-me-int/{me-int:int}")]
		public static async Task GetRouteWithPathParameterMeInt(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			var me = context.Request.PathParameters.GetParameterValue<int>("me-int");
			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", To = me, Success = true });
		}

		[HttpOptions("/options/cors-request")]
		public static async Task OptionsCorsRoute(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			context.Response.AddHeader(HttpHeaders.Origin, "*");
			context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, string.Join(" ", HttpHeaders.ContentType, HttpHeaders.Accept, HttpHeaders.XRequestedWith));
			context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, string.Join(" ", HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete));
			context.Response.StatusCode = HttpStatusCode.NoContent;
			await context.Response.SendAsync();
		}

		[HttpGet("/get/compressed-response")]
		public static async Task GetCompressRoute(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();
			
			await context.Response.WriteTextAsync(string.Concat(Enumerable.Repeat("Apes!", 10000)));
		}

		[HttpGet("/get/send-response")]
		public static async Task GetSendResponse(HttpContext context)
		{
			var service = context.GetGreetingsService();
			var greetings = service.Greet();

			await context.Response.WriteJsonAsync(new { Message = greetings, From = "Everest", Success = true });
			await context.Response.SendAsync();
		}

		[HttpGet("/get/empty-response")]
		public static async Task GetEmptyResponse(HttpContext context)
		{
			await context.Response.SendAsync();
		}

		[HttpGet("/get/exception-response")]
		public static Task GetExceptionResponse(HttpContext context)
		{
			throw new InvalidOperationException("Something went wrong ;(");
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