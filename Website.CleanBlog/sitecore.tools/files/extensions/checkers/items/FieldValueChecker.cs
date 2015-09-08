// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    [Export(typeof(IChecker))]
    public class FieldValueChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items.OfType<Item>().Where(i => !i.IsExternalReference))
            {
                foreach (var field in item.Fields)
                {
                    if (!field.IsResolved)
                    {
                        field.Resolve();
                    }

                    foreach (var diagnostic in field.Diagnostics)
                    {
                        switch (diagnostic.Severity)
                        {
                            case Severity.Error:
                                context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                                break;
                            case Severity.Warning:
                                context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                                break;
                            default:
                                context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Position);
                                break;
                        }
                    }
                }
            }
        }
    }
}
