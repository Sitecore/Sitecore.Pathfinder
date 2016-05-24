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
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Building
{
    [Export]
    public class Builder : TaskRunnerBase
    {
        [ImportingConstructor]
        public Builder([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem, [NotNull, ItemNotNull, ImportMany] IEnumerable<ITask> tasks, [NotNull] ExportFactory<IBuildContext> buildContextFactory) : base(configuration, tasks)
        {
            CompositionService = compositionService;
            FileSystem = fileSystem;
            BuildContextFactory = buildContextFactory;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected ExportFactory<IBuildContext> BuildContextFactory { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public override int Start()
        {
            var context = BuildContextFactory.New();

            RegisterProjectDirectory(context);

            RunTasks(context);

            return context.ErrorCode;
        }

        protected virtual void RegisterProjectDirectory([NotNull] IBuildContext context)
        {
            // registering a project directory in the website Data Folder allows the website and other tools
            // to locate the project 
            var dataFolder = context.Configuration.GetString(Constants.Configuration.DataFolderDirectory);
            if (!FileSystem.DirectoryExists(dataFolder))
            {
                return;
            }

            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");
            FileSystem.CreateDirectory(pathfinderFolder);

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");

            var xml = FileSystem.FileExists(fileName) ? FileSystem.ReadAllText(fileName) : "<projects />";

            var root = xml.ToXElement() ?? "<projects />".ToXElement();
            if (root == null)
            {
                // silent
                return;
            }

            // check if already registered
            if (root.Elements().Any(e => string.Equals(e.GetAttributeValue("projectdirectory"), context.Project.ProjectDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            root.Add(new XElement("project", new XAttribute("toolsdirectory", context.ToolsDirectory), new XAttribute("projectdirectory", context.Project.ProjectDirectory)));

            if (root.Document != null)
            {
                root.Document.Save(fileName);
            }
        }
    }
}
