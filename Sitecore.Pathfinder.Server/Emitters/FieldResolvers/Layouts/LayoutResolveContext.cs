// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.FieldResolvers.Layouts
{
    public class LayoutResolveContext
    {
        public LayoutResolveContext(Field field, [NotNull] IProject project, [NotNull] ITextSnapshot snapshot, string databaseName)
        {
            Field = field;
            Project = project;
            Snapshot = snapshot;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public Field Field { get; }

        [NotNull]
        public IProject Project { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }
    }
}
