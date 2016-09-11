// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Threading;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;
using Sitecore.Pathfinder.WebServer.Web;

namespace Sitecore.Pathfinder.WebServer.Tasks
{
    public class StartWebsite : WatchTaskBase
    {
        [NotNull]
        private readonly HttpListener _listener = new HttpListener();

        [CanBeNull]
        private IProject _project;

        [ImportingConstructor]
        public StartWebsite([NotNull] IConsoleService console, [NotNull] IProjectService projectService, [NotNull] IFileSystemService fileSystem, [ImportMany(typeof(IHttpListenerHandler)), NotNull, ItemNotNull] IEnumerable<IHttpListenerHandler> handlers) : base(console, "start-website")
        {
            ProjectService = projectService;
            FileSystem = fileSystem;
            Handlers = handlers;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IHttpListenerHandler> Handlers { get; }

        [NotNull]
        protected string ProjectDirectory { get; set; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1000, Texts.Starting_website___);

            var port = context.Configuration.GetInt(Constants.Configuration.StartWebsite.Port, 3000);
            var prefix = "http://localhost:" + port + "/";

            context.Trace.TraceInformation(Msg.D1000, @"    " + prefix);

            ProjectDirectory = context.ProjectDirectory;
            LoadProject();

            var include = context.Configuration.GetString(Constants.Configuration.StartWebsite.Include, "**");
            var exclude = context.Configuration.GetString(Constants.Configuration.StartWebsite.Exclude, "**");
            var pathMatcher = new PathMatcher(include, exclude);

            StartWatching(context.ProjectDirectory, pathMatcher);

            _listener.Prefixes.Add(prefix);
            _listener.Start();

            ThreadPool.QueueUserWorkItem(state =>
            {
                while (_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(ProcessRequest, _listener.GetContext());
                }
            });

            Console.WriteLine(Texts.Type__q__to_quit___);
            string input;
            do
            {
                input = Console.ReadLine();
            }
            while (!string.Equals(input, "q", StringComparison.OrdinalIgnoreCase));

            _listener.Stop();
            _listener.Close();
        }

        protected virtual void ProcessRequest([NotNull] object state)
        {
            var project = _project;
            var listenerContext = (HttpListenerContext)state;

            try
            {
                if (project == null)
                {
                    NotFound(listenerContext, "Project not loaded");
                    return;
                }

                var handled = false;
                foreach (var handler in Handlers)
                {
                    if (!handler.CanProcessRequest(listenerContext, project))
                    {
                        continue;
                    }

                    handler.ProcessRequest(listenerContext, project);
                    handled = true;
                    break;
                }

                if (!handled)
                {
                    NotFound(listenerContext, "Not found");
                }
            }
            finally
            {
                listenerContext.Response.OutputStream.Close();
            }
        }

        protected virtual void LoadProject()
        {
            Console.WriteLine(Texts.Loading_project___);
            _project = ProjectService.LoadProjectFromNewHost(ProjectDirectory);
        }

        protected virtual void NotFound([NotNull] HttpListenerContext listenerContext, [NotNull] string message)
        {
            listenerContext.Response.StatusCode = 404;
            var buffer = Encoding.UTF8.GetBytes(message);
            listenerContext.Response.ContentLength64 = buffer.Length;
            listenerContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        protected override void OnChange(object state)
        {
            LoadProject();
        }
    }
}
