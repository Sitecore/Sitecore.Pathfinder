// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.References
{
    public interface IReferenceParserService
    {
        bool IsIgnoredReference([NotNull] string referenceText);

        [NotNull, ItemNotNull]
        IEnumerable<IReference> ParseReferences([NotNull] Field field);

        [NotNull, ItemNotNull]
        IEnumerable<IReference> ParseReferences<T>([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<T> sourceProperty);

        [NotNull, ItemNotNull]
        IEnumerable<IReference> ParseReferences([NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode);
    }
}
