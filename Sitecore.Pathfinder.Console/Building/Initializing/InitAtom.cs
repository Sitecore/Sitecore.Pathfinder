// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;

namespace Sitecore.Pathfinder.Building.Initializing
{
    [Export(typeof(ITask))]
    public class InitAtom : TaskBase
    {
        public InitAtom() : base("init-atom")
        {
        }

        public override void Run(IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\atom\\*");

            context.FileSystem.XCopy(sourceDirectory, context.SolutionDirectory);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates a new Atom project.");
        }
    }
}
