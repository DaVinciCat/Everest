﻿using Everest.Services;
using System;

namespace Everest.Files
{
	public class StaticFilesProviderConfigurator : ServiceConfigurator<StaticFilesProvider>
	{
		public StaticFilesProvider FilesProvider => Service;

		public StaticFilesProviderConfigurator(StaticFilesProvider filesProvider, IServiceProvider services)
			: base(filesProvider, services)
		{

		}

        public StaticFilesProviderConfigurator UsePhysicalPath(string physicalPath)
        {
            FilesProvider.PhysicalPath = physicalPath;
            return this;
        }
    }
}
