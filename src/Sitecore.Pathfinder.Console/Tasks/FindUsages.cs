// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class FindUsages : QueryBuildTaskBase
    {
        [ImportingConstructor]
        public FindUsages() : base("find-usages")
        {
        }

        public override void Run(IBuildContext context)
        {
            var qualifiedName = context.Configuration.GetCommandLineArg(0);
            if (string.IsNullOrEmpty(qualifiedName))
            {
                context.Trace.WriteLine(Texts.You_must_specific_the___name_argument);
                return;
            }

            var project = context.LoadProject();
            var references = project.Index.FindUsages(qualifiedName).ToList();

            Display(context, references);

            context.Trace.WriteLine(Texts.Found__ + references.Count);
        }
    }
}
