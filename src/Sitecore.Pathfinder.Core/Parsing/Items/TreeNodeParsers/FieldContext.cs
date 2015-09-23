using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public class FieldContext
    {
        [NotNull]
        public SourceProperty<string> LanguageProperty { get; } = new SourceProperty<string>("Language", string.Empty);

        [NotNull]
        public SourceProperty<int> VersionProperty { get; } = new SourceProperty<int>("Number", 0);
    }
}