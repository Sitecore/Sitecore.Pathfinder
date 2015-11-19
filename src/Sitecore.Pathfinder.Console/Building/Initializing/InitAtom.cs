// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class InitAtom : BuildTaskBase
    {
        public InitAtom() : base("init-atom")
        {
        }

        public override void Run(IBuildContext context)
        {
            var zipFileName = Path.Combine(context.ToolsDirectory, "files\\editors\\Atom.zip");

            context.FileSystem.Unzip(zipFileName, context.ProjectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Atom project.");
        }
    }
}
