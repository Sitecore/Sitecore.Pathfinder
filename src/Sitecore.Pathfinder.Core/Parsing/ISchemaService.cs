// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public interface ISchemaService
    {
        bool ValidateSnapshotSchema([NotNull] IParseContext context, [NotNull] ITextSnapshot textSnapshot);

        bool ValidateTextNodeSchema([NotNull] ITextNode textNode);

        bool ValidateTextNodeSchema([NotNull] ITextNode textNode, [NotNull] string textNodeName);
    }
}
