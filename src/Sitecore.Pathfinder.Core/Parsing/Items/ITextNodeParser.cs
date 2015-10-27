// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    [InheritedExport]
    public interface ITextNodeParser
    {
        double Priority { get; }

        bool CanParse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);

        void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);
    }
}
