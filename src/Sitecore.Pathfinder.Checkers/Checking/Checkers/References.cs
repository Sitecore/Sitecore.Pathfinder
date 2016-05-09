// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class References : Check
    {
        [Export("Check")]
        public IEnumerable<Diagnostic> ReferenceNotFound(ICheckerContext context)
        {
            return 
                from projectItem in context.Project.ProjectItems
                from reference in projectItem.References
                where 
                    !reference.IsValid
                select 
                    Error(Msg.C1000, "Reference not found", reference.SourceProperty.GetValue(), reference.SourceProperty);
        }
    }
}
