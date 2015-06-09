// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker))]
    public class GuidChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var projectItem1 in context.Project.Items)
            {
                foreach (var projectItem2 in context.Project.Items)
                {
                    if (projectItem1 == projectItem2)
                    {
                        continue;
                    }

                    if (projectItem1.Guid != projectItem2.Guid)
                    {
                        continue;
                    }

                    context.Trace.TraceError(Texts.Unique_ID_clash, projectItem1.QualifiedName);
                    context.IsDeployable = false;
                }
            }
        }
    }
}
