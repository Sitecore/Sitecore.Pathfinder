// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Parsing
{
    public interface IParser
    {
        double Priority { get; }

        bool CanParse([NotNull] IParseContext context);

        void Parse([NotNull] IParseContext context);
    }
}
