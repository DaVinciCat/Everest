using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Middlewares
{
	public class AggregateMiddleware : Middleware
	{
		private readonly List<IMiddleware> collection = new List<IMiddleware>();

		public AggregateMiddleware()
		{
			
		}

		public AggregateMiddleware(IMiddleware[] middleware)
		{
			if (middleware == null) 
				throw new ArgumentNullException(nameof(middleware));

			foreach (var mw in middleware)
			{
				AddMiddleware(mw);
			}
		}

		public void AddMiddleware(IMiddleware middleware)
		{
			if (middleware == null) 
				throw new ArgumentNullException(nameof(middleware));

			if (collection.Count > 0)
			{
				var last = collection.Count - 1;
				collection[last].SetNextMiddleware(middleware);
			}

			collection.Add(middleware);
		}
		
		public override async Task InvokeAsync(HttpContext request)
		{
			if (request == null) 
				throw new ArgumentNullException(nameof(request));

			if (collection.Count <= 0)
				throw new InvalidOperationException("No middleware added");
			
			await collection[0].InvokeAsync(request);
		}
	}
}
