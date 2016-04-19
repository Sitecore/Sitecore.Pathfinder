// © 2015 Sitecore Corporation A/S. All rights reserved.

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
    public class WatchProject : RequestBuildTaskBase
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
            Context = context;
            Context.IsAborted = true;

            var include = context.Configuration.GetString(Constants.Configuration.WatchProjectInclude, "**");
            var exclude = context.Configuration.GetString(Constants.Configuration.WatchProjectExclude, "**");
            _pathMatcher = new PathMatcher(include, exclude);

            _publishDatabase = context.Configuration.GetBool(Constants.Configuration.WatchProjectPublishDatabase, true);

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

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Watches the project directory and install changes immediately.");
            helpWriter.Remarks.WriteLine("Settings:");
            helpWriter.Remarks.WriteLine("  watch-project:include - Specifies which files to look for");
            helpWriter.Remarks.WriteLine("  watch-project:exclude - Specifies which files to ignore");
            helpWriter.Remarks.WriteLine("  watch-project:publish-database - Indicates if the database should published after installing the project");
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

            var url = MakeWebsiteTaskUrl(Context, "InstallProject");
            Post(Context, url);
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

            var url = MakeUrl(Context, Context.Configuration.GetString(Constants.Configuration.PublishUrl), queryStringParameters);
            Post(Context, url);
        }
    }
}
