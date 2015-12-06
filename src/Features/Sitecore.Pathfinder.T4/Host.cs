// © 2015 Sitecore Corporation A/S. All rights reserved.

using Mono.TextTemplating;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

// ReSharper disable once CheckNamespace
namespace T4
{
    public class Host : TemplateGenerator
    {
        public Host([NotNull] IBuildContext context, [NotNull] IProject project)
        {
            Context = context;
            Project = project;
        }

        [NotNull]
        public IBuildContext Context { get; }

        [CanBeNull]
        public IProjectItem Item { get; internal set; }

        [NotNull]
        public IProject Project { get; }
    }
}
