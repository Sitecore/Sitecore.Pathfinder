// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Querying
{
    [Export(typeof(IQueryService))]
    public class QueryService : IQueryService
    {
        public IProjectItem FindProjectItem(IProject project, string qualifiedName)
        {
            return project.FindQualifiedItem<IProjectItem>(qualifiedName);
        }

        public IEnumerable<IReference> FindUsages(IProject project, string qualifiedName)
        {
            var target = FindProjectItem(project, qualifiedName);
            if (target == null)
            {
                yield break;
            }

            foreach (var projectItem in project.ProjectItems)
            {
                foreach (var reference in projectItem.References)
                {
                    var i = reference.Resolve();
                    if (i == target)
                    {
                        yield return reference;
                    }
                }
            }
        }
    }
}
