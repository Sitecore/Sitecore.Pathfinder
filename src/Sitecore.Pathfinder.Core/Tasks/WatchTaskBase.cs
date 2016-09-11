// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public abstract class WatchTaskBase : WebBuildTaskBase
    {
        [NotNull]
        private static readonly object SyncObject = new object();

        [NotNull]
        private readonly Timer _timer;

        [NotNull]
        private FileSystemWatcher _fileWatcher;

        [NotNull]
        private PathMatcher _pathMatcher;

        [ImportingConstructor]
        protected WatchTaskBase([NotNull] IConsoleService console, [NotNull] string taskName) : base(taskName)
        {
            Console = console;

            _timer = new Timer(Process, null, Timeout.Infinite, Timeout.Infinite);
        }

        protected bool Busy { get; set; }

        [NotNull]
        protected IConsoleService Console { get; }

        protected bool RestartTimer { get; set; }

        protected virtual void FileChanged([NotNull] object sender, [NotNull] FileSystemEventArgs fileSystemEventArgs)
        {
            if (_pathMatcher.IsMatch(fileSystemEventArgs.FullPath))
            {
                StartTimer();
            }
        }

        protected abstract void OnChange([CanBeNull] object state);

        protected virtual void StartTimer()
        {
            if (Busy)
            {
                RestartTimer = true;
                return;
            }

            lock (SyncObject)
            {
                _timer.Change(TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(-1));
            }
        }

        protected void StartWatching([NotNull] string directory, [NotNull] PathMatcher pathMatcher)
        {
            _pathMatcher = pathMatcher;
            _fileWatcher = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileWatcher.Changed += FileChanged;
            _fileWatcher.Deleted += FileChanged;
            _fileWatcher.Renamed += FileChanged;
            _fileWatcher.Created += FileChanged;
            _fileWatcher.Created += FileChanged;

            _fileWatcher.EnableRaisingEvents = true;
        }

        protected virtual void StopTimer()
        {
            lock (SyncObject)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void Process([CanBeNull] object state)
        {
            Busy = true;
            StopTimer();

            try
            {
                OnChange(state);
            }
            finally
            {
                Busy = false;

                if (RestartTimer)
                {
                    StartTimer();
                }
            }
        }
    }
}
