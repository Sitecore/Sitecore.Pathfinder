// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class InitAtom : BuildTaskBase
    {
        [ImportingConstructor]
        public InitAtom([NotNull] IFileSystemService fileSystem) : base("init-atom")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var zipFileName = Path.Combine(context.ToolsDirectory, "files\\editors\\Atom.zip");

            FileSystem.Unzip(zipFileName, context.Project.ProjectDirectory);
        }
    }
}
