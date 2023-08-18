using Everest.Services;
using System;

namespace Everest.Files
{
	public class StaticFilesProviderConfigurator : ServiceConfigurator<IStaticFilesProvider>
	{
		public IStaticFilesProvider FilesProvider => Service;

		public StaticFilesProviderConfigurator(IStaticFilesProvider filesProvider, IServiceProvider services)
			: base(filesProvider, services)
		{

		}
	}

	public class DefaultStaticFilesProviderConfigurator : StaticFilesProviderConfigurator
	{
		public new StaticFilesProvider FilesProvider { get; }

		public DefaultStaticFilesProviderConfigurator(StaticFilesProvider filesProvider, IServiceProvider services)
			: base(filesProvider, services)
		{
			FilesProvider = filesProvider;
		}

		public StaticFilesProviderConfigurator UsePhysicalPath(string physicalPath)
		{
			FilesProvider.PhysicalPath = physicalPath;
			return this;
		}
	}
}
