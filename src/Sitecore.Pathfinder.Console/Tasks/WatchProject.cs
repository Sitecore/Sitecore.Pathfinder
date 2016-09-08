// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class WatchProject : WebBuildTaskBase
    {
        [NotNull]
        private static readonly object SyncObject = new object();

        [NotNull]
        private readonly Timer _timer;

        [NotNull]
        private FileSystemWatcher _fileWatcher;

        [NotNull]
        private PathMatcher _pathMatcher;

        private bool _publishDatabase;

        private bool _resetWebsite;

        [ImportingConstructor]
        public WatchProject([NotNull] IConsoleService console) : base("watch-project")
        {
            Console = console;

            _timer = new Timer(InstallProject, null, Timeout.Infinite, Timeout.Infinite);
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IBuildContext Context { get; set; }

        protected bool Installing { get; set; }

        protected bool RestartTimer { get; set; }

        public override void Run(IBuildContext context)
        {
            if (!IsProjectConfigured(context))
            {
                return;
            }

            Context = context;

            var include = context.Configuration.GetString(Constants.Configuration.WatchProject.Include, "**");
            var exclude = context.Configuration.GetString(Constants.Configuration.WatchProject.Exclude, "**");
            _pathMatcher = new PathMatcher(include, exclude);

            _resetWebsite = context.Configuration.GetBool(Constants.Configuration.WatchProject.ResetWebsite, true);
            _publishDatabase = context.Configuration.GetBool(Constants.Configuration.WatchProject.PublishDatabase, true);

            _fileWatcher = new FileSystemWatcher(context.ProjectDirectory)
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

            Console.WriteLine(Texts.Type__q__to_quit___);

            string input;
            do
            {
                input = Console.ReadLine();
            }
            while (!string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));
        }

        protected virtual void FileChanged([NotNull] object sender, [NotNull] FileSystemEventArgs fileSystemEventArgs)
        {
            if (_pathMatcher.IsMatch(fileSystemEventArgs.FullPath))
            {
                StartTimer();
            }
        }

        protected virtual void InstallProject([CanBeNull] object state)
        {
            Installing = true;
            StopTimer();

            try
            {
                Console.WriteLine();

                if (_resetWebsite)
                {
                    ResetWebsite();
                }

                InstallProject();

                if (_publishDatabase)
                {
                    PublishDatabase();
                }

                Console.WriteLine(Texts.Done);
            }
            finally
            {
                Installing = false;

                if (RestartTimer)
                {
                    StartTimer();
                }
            }
        }

        protected virtual void StartTimer()
        {
            if (Installing)
            {
                RestartTimer = true;
                return;
            }

            lock (SyncObject)
            {
                _timer.Change(TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(-1));
            }
        }

        protected virtual void StopTimer()
        {
            lock (SyncObject)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void InstallProject()
        {
            Console.WriteLine(Texts.Installing_project___);

            var webRequest = GetWebRequest(Context).AsTask("InstallProject");

            Post(Context, webRequest);
        }

        private void PublishDatabase()
        {
            if (string.Equals(Context.Project.Options.DatabaseName, "core", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Console.WriteLine(Texts.Publishing___);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = "i",
                ["db"] = Context.Project.Options.DatabaseName
            };

            var webRequest = GetWebRequest(Context).WithUrl(Context.Configuration.GetString(Constants.Configuration.PublishDatabases.PublishUrl)).WithQueryString(queryStringParameters);

            Post(Context, webRequest);
        }

        private void ResetWebsite()
        {
            Console.WriteLine(Texts.Resetting_website___);

            var webRequest = GetWebRequest(Context).AsTask("ResetWebsite");

            Post(Context, webRequest);
        }
    }
}
