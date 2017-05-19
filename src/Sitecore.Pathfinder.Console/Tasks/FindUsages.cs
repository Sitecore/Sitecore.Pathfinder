// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
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
            var references = GetUsages(project, qualifiedName).ToArray();

            Display(context, references);

            context.Trace.WriteLine(Texts.Found__ + references.Length);
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<IReference> GetUsages([NotNull] IProject project, [NotNull] string qualifiedName)
        {
            var target = project.Indexes.FindQualifiedItem<IProjectItem>(qualifiedName);
            if (target == null)
            {
                yield break;
            }

            foreach (var item in project.Indexes.Items)
            {
                foreach (var reference in item.References)
                {
                    var i = reference.Resolve();
                    if (i == target)
                    {
                        yield return reference;
                    }
                }
            }

            foreach (var item in project.Indexes.Templates)
            {
                foreach (var reference in item.References)
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
