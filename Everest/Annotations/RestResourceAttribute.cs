﻿using System;

namespace Everest.Annotations
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RestResourceAttribute : Attribute
	{
		public string RoutePrefix { get; }

		public RestResourceAttribute(string routePrefix = null)
		{
			RoutePrefix = routePrefix;
		}
	}
}
