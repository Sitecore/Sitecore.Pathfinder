// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Querying
{
    [Export(typeof(IQueryService))]
    public class QueryService : IQueryService
    {
        public IProjectItem FindProjectItem(IProject project, string qualifiedName)
        {
            return project.Items.FirstOrDefault(i => string.Compare(i.QualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public IEnumerable<IReference> FindUsages(IProject project, string qualifiedName)
        {
            foreach (var projectItem in project.Items)
            {
                foreach (var reference in projectItem.References)
                {
                    if (string.Compare(reference.TargetQualifiedName, qualifiedName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        yield return reference;
                    }
                }
            }
        }
    }
}
