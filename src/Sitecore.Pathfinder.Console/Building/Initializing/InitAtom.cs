// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class InitAtom : TaskBase
    {
        public InitAtom() : base("init-atom")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\atom\\*");

            context.FileSystem.XCopy(sourceDirectory, context.ProjectDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Atom project.");
        }
    }
}
