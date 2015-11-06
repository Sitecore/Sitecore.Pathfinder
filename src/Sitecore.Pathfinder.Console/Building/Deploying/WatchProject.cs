// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class WatchProject : RequestTaskBase
    {
        [NotNull]
        private static readonly object _syncObject = new object();

        [NotNull]
        private FileSystemWatcher _fileWatcher;

        [NotNull]
        Timer _timer;

        public WatchProject() : base("watch-project")
        {
            _timer = new Timer(InstallProject, null, Timeout.Infinite, Timeout.Infinite);
        }

        [NotNull]
        protected IBuildContext Context { get; set; }

        protected bool Installing { get; set; }

        protected bool RestartTimer { get; set; }

        public override void Run(IBuildContext context)
        {
            Context = context;
            Context.IsAborted = true;

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

            Console.WriteLine("Type 'q' to quit...");

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
        }

        protected virtual void FileChanged([NotNull] object sender, [NotNull] FileSystemEventArgs fileSystemEventArgs)
        {
            StartTimer();
        }

        protected virtual void InstallProject([CanBeNull] object state)
        {
            Installing = true;
            StopTimer();

            try
            {
                Console.WriteLine();

                InstallProject();

                PublishDatabase();

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

            lock (_syncObject)
            {
                _timer.Change(TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(-1));
            }
        }

        protected virtual void StopTimer()
        {
            lock (_syncObject)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void InstallProject()
        {
            Console.WriteLine("Installing project...");

            var queryStringParameters = new Dictionary<string, string>
            {
                ["td"] = Context.Configuration.GetString(Constants.Configuration.ToolsDirectory),
                ["pd"] = Context.ProjectDirectory
            };

            var url = MakeWebApiUrl(Context, "InstallProject", queryStringParameters);
            Request(Context, url);
        }

        private void PublishDatabase()
        {
            if (string.Equals(Context.Project.Options.DatabaseName, "core", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Console.WriteLine("Publishing database...");

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = "i",
                ["db"] = Context.Project.Options.DatabaseName
            };

            var url = MakeUrl(Context, Context.Configuration.GetString(Constants.Configuration.PublishUrl), queryStringParameters);
            Request(Context, url);
        }
    }
}
