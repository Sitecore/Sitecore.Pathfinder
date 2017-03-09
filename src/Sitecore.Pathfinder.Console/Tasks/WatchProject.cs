// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class WatchProject : WatchTaskBase
    {
        private bool _publishDatabase;

        private bool _resetWebsite;

        [ImportingConstructor]
        public WatchProject([NotNull] IConsoleService console) : base(console, "watch-project")
        {
        }

        [NotNull]
        protected IBuildContext Context { get; set; }

        public override void Run(IBuildContext context)
        {
            if (!IsProjectConfigured(context))
            {
                return;
            }

            Context = context;

            var include = context.Configuration.GetString(Constants.Configuration.WatchProject.Include, "**");
            var exclude = context.Configuration.GetString(Constants.Configuration.WatchProject.Exclude, "**");
            var pathMatcher = new PathMatcher(include, exclude);

            _resetWebsite = context.Configuration.GetBool(Constants.Configuration.WatchProject.ResetWebsite, true);
            _publishDatabase = context.Configuration.GetBool(Constants.Configuration.WatchProject.PublishDatabase, true);

            StartWatching(context.ProjectDirectory, pathMatcher);

            Console.WriteLine(Texts.Type__q__to_quit___);

            string input;
            do
            {
                input = Console.ReadLine();
            }
            while (!string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));
        }

        protected virtual void InstallProject()
        {
            Console.WriteLine(Texts.Installing_project___);

            var webRequest = GetWebRequest(Context).AsTask("InstallProject");

            Post(Context, webRequest);
        }

        protected override void OnChange(object state)
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

        private void PublishDatabase()
        {
            var project = Context.LoadProject();

            if (string.Equals(project.Options.DatabaseName, "core", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Console.WriteLine(Texts.Publishing___);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = "i",
                ["db"] = project.Options.DatabaseName
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
