﻿using Everest.Http;

namespace Everest.Middleware
{
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
}
