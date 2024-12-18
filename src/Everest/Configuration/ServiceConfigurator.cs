﻿using System;

namespace Everest.Configuration
{
	public abstract class ServiceConfigurator<TService>
	    where TService : class
	{
		protected TService Service { get; }

		public IServiceProvider Services { get; }

		protected ServiceConfigurator(TService service, IServiceProvider services)
		{
            Service = service ?? throw new ArgumentNullException(nameof(service));
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
	}
}
