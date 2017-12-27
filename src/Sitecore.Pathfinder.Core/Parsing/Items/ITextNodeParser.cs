// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public interface ITextNodeParser
    {
        double Priority { get; }

        [CanBeNull]
        ISchemaService SchemaService { get; }

        bool CanParse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);

        void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);
    }
}
