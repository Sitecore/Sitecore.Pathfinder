// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Building
{
    [Export]
    public class BuildRunner : TaskRunnerBase
    {
        [ImportingConstructor]
        public BuildRunner([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] ITraceService trace, [ImportMany, NotNull, ItemNotNull] IEnumerable<ITask> tasks) : base(compositionService, configuration, console, trace, tasks)
        {
        }

        public override int Start()
        {
            var context = CompositionService.Resolve<IBuildContext>();

            RegisterProjectDirectory(context);

            RunTasks(context);

            if (context.IsAborted)
            {
                return 0;
            }

            var errorCode = context.Project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;
            if (!context.DisplayDoneMessage)
            {
                return errorCode;
            }

            Console.Write(Texts.Ducats___0_, context.Project.Ducats.ToString("#,##0"));
            if (AppService.Stopwatch != null)
            {
                AppService.Stopwatch.Stop();
                Console.Write(", time: {0}ms", AppService.Stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0"));
            }

            Console.WriteLine();
            Console.WriteLine(Texts.Done);

            if (Configuration.GetBool("pause"))
            {
                Console.ReadLine();
            }

            return errorCode;
        }

        protected virtual void RegisterProjectDirectory([NotNull] IBuildContext context)
        {
            // registering a project directory in the website Data Folder allows the website and other tools
            // to locate the project 
            var dataFolder = context.Configuration.GetString(Constants.Configuration.DataFolderDirectory);
            if (!context.FileSystem.DirectoryExists(dataFolder))
            {
                return;
            }

            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");
            context.FileSystem.CreateDirectory(pathfinderFolder);

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");

            var xml = context.FileSystem.FileExists(fileName) ? context.FileSystem.ReadAllText(fileName) : "<projects />";

            var root = xml.ToXElement() ?? "<projects />".ToXElement();
            if (root == null)
            {
                // silent
                return;
            }

            // check if already registered
            if (root.Elements().Any(e => string.Equals(e.GetAttributeValue("projectdirectory"), context.ProjectDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            root.Add(new XElement("project", new XAttribute("toolsdirectory", context.ToolsDirectory), new XAttribute("projectdirectory", context.ProjectDirectory)));

            if (root.Document != null)
            {
                root.Document.Save(fileName);
            }
        }
    }
}
