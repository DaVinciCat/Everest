using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Everest.Http;

namespace Everest.Files
{
	public class StaticFilesProvider : IStaticFilesProvider
	{
		#region Logger

		public ILogger<StaticFilesProvider> Logger { get; }

		#endregion

		#region Files
		
		public string DefaultPhysicalPath => "public";

		public string PhysicalPath
		{
			get => fileWatcher.Path;
			set
			{
				if(!Directory.Exists(value))
				{
					Logger.LogWarning($"Static files directory does not exist: {new {PhysicalPath = value}}");
					return;
				}

				fileWatcher.Path = value;
				fileWatcher.NotifyFilter = NotifyFilters.Attributes
				                           | NotifyFilters.CreationTime
				                           | NotifyFilters.DirectoryName
				                           | NotifyFilters.FileName
				                           | NotifyFilters.LastAccess
				                           | NotifyFilters.LastWrite
				                           | NotifyFilters.Security
				                           | NotifyFilters.Size;

				fileWatcher.IncludeSubdirectories = true;
				fileWatcher.EnableRaisingEvents = true;

				files.Clear();
				foreach (var file in Directory.EnumerateFiles(PhysicalPath, "*.*", SearchOption.AllDirectories))
				{
					AddFile(file);
				}
			}
		}

		public string[] Files => files.ToArray();

		private readonly HashSet<string> files = new();

		private void AddFile(string filePath)
		{
			files.Add(filePath);
		}

		private void RemoveFile(string filePath)
		{
			files.Remove(filePath);
		}
		
		private string RequestPathToPhysicalPath(string filePath)
		{
			return Path.Combine(PhysicalPath, filePath.Trim('/').Replace("/", "\\"));
		}

		public bool HasFile(HttpRequest request)
		{
			return files.Contains(RequestPathToPhysicalPath(request.Path));
		}

		public bool TryGetFile(HttpRequest request, out FileInfo file)
		{
			if (HasFile(request))
			{
				var physicalPath = RequestPathToPhysicalPath(request.Path);
				file = new FileInfo(physicalPath);
				return true;
			}

			file = null;
			return false;
		}

		#endregion

		#region FileWatcher

		private readonly FileSystemWatcher fileWatcher = new();

		private void OnError(object sender, ErrorEventArgs e)
		{
			var ex = e.GetException();
			Logger.LogError(ex, $"File watcher error occurred {new { Error = ex.Message }}");
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			if (files.Contains(e.OldFullPath))
			{
				RemoveFile(e.OldFullPath);
			}

			if (!files.Contains(e.FullPath))
			{
				AddFile(e.FullPath);
			}
		}

		private void OnDeleted(object sender, FileSystemEventArgs e)
		{
			if (files.Contains(e.FullPath))
			{
				RemoveFile(e.FullPath);
			}
		}

		private void OnCreated(object sender, FileSystemEventArgs e)
		{
			if (!files.Contains(e.FullPath))
			{
				AddFile(e.FullPath);
			}
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Changed)
			{
				return;
			}

			if (!files.Contains(e.FullPath))
			{
				AddFile(e.FullPath);
			}
		}

		#endregion
		
		#region Subscribe

		private void Subscribe(bool subscribe)
		{
			if (subscribe)
			{
				fileWatcher.Changed += OnChanged;
				fileWatcher.Created += OnCreated;
				fileWatcher.Deleted += OnDeleted;
				fileWatcher.Renamed += OnRenamed;
				fileWatcher.Error += OnError;
			}
			else
			{
				fileWatcher.Changed -= OnChanged;
				fileWatcher.Created -= OnCreated;
				fileWatcher.Deleted -= OnDeleted;
				fileWatcher.Renamed -= OnRenamed;
				fileWatcher.Error -= OnError;
			}
		}

		#endregion

		#region Ctor

		public StaticFilesProvider(ILogger<StaticFilesProvider> logger)
		{
			Logger = logger;

			if (Directory.Exists(DefaultPhysicalPath))
			{
				PhysicalPath = DefaultPhysicalPath;
			}

			Subscribe(true);
		}

		#endregion

		#region IDisposable

		private bool disposed;

		public void Dispose()
		{
			if (disposed)
				return;

			Subscribe(false);
			fileWatcher.Dispose();
			disposed = true;
		}

		#endregion
	}
}
