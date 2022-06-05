using System.Collections.Generic;
using Everest.Http;

namespace Everest.Utils
{
	public interface IMiddleware
	{
		void SetNextMiddleware(IMiddleware next);

		void Invoke(HttpContext context);
	}

	public abstract class MiddlewareBase : IMiddleware
	{
		public IMiddleware Next;

		protected bool HasNext => Next != null;

		public void SetNextMiddleware(IMiddleware next)
		{
			Next = next;
		}

		public abstract void Invoke(HttpContext context);
	}

	public class AggregateMiddleware : MiddlewareBase
	{
		private readonly List<IMiddleware> collection = new();
		
		public void AddMiddleware(IMiddleware middleware)
		{
			if (collection.Count > 0)
			{
				var last = collection.Count - 1;
				collection[last].SetNextMiddleware(middleware);
			}

			collection.Add(middleware);
		}
		
		public override void Invoke(HttpContext request)
		{
			if (collection.Count > 0)
			{
				collection[0].Invoke(request);
			}
		}
	}
}
