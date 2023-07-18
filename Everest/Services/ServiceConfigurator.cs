using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everest.Services
{
	public abstract class ServiceConfigurator<TService>
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
