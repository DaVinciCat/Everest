using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Everest.Files
{
	public class StaticFilesProvider : IStaticFilesProvider
	{
		#region Logger

		public ILogger<StaticFilesProvider> Logger { get; }

		#endregion

		#region Sync

		private readonly object sync = new object();

		#endregion

		#region Files
		
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

				ClearFiles();
				foreach (var file in Directory.EnumerateFiles(PhysicalPath, "*.*", SearchOption.AllDirectories))
				{
					AddFile(file);
				}
			}
		}

		public string[] Files
		{
			get
			{
				lock (sync)
				{
					return files.ToArray();
				}
			}
		}

		private readonly HashSet<string> files = new HashSet<string>();
		
        private void AddFile(string filePath)
		{
			lock (sync)
			{
				files.Add(filePath);
			}
		}

		private void RemoveFile(string filePath)
		{
			lock (sync)
			{
				files.Remove(filePath);
			}
		}

		private void ClearFiles()
		{
			lock (sync)
			{
				files.Clear();
			}
		}

		private bool HasFile(string filePath)
		{
			lock (sync)
			{
				return files.Contains(filePath);
			}
		}
		
		public bool TryGetFile(string filePath, out FileInfo file)
        {
            filePath = Path.Combine(PhysicalPath, filePath);
			if (HasFile(filePath))
			{
				file = new FileInfo(filePath);
				return true;
			}

			file = null;
			return false;
		}
       
		#endregion

		#region FileWatcher

		private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();

		private void OnError(object sender, ErrorEventArgs e)
		{
			var ex = e.GetException();
			Logger.LogError(ex, $"File watcher error occurred {new { Error = ex.Message }}");
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			if (HasFile(e.OldFullPath))
			{
				RemoveFile(e.OldFullPath);
			}

			if (!HasFile(e.FullPath))
			{
				AddFile(e.FullPath);
			}
		}

		private void OnDeleted(object sender, FileSystemEventArgs e)
		{
			if (HasFile(e.FullPath))
			{
				RemoveFile(e.FullPath);
			}
		}

		private void OnCreated(object sender, FileSystemEventArgs e)
		{
			if (!HasFile(e.FullPath))
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

			if (!HasFile(e.FullPath))
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
		
		public StaticFilesProvider(string physicalPath, ILogger<StaticFilesProvider> logger)
		{
			Logger = logger;

			if (Directory.Exists(physicalPath))
			{
				PhysicalPath = physicalPath;
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
			ClearFiles();
			disposed = true;
		}

		#endregion
	}
}
