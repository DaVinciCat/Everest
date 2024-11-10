using System;
using Everest.Configuration;

namespace Everest.StaticFiles
{
	public class StaticFilesProviderConfigurator : ServiceConfigurator<StaticFilesProvider>
	{
		public StaticFilesProvider StaticFilesProvider => Service;

		public StaticFilesProviderConfigurator(StaticFilesProvider staticFilesProvider, IServiceProvider services)
			: base(staticFilesProvider, services)
		{

		}

        public StaticFilesProviderConfigurator UsePhysicalPath(string physicalPath)
        {
            StaticFilesProvider.PhysicalPath = physicalPath;
            return this;
        }
    }
}
