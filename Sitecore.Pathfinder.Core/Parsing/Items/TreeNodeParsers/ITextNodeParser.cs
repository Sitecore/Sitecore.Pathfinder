// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public interface ITextNodeParser
    {
        double Priority { get; }

        bool CanParse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);

        void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode);
    }
}
