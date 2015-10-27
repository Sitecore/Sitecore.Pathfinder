// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Parsing
{
    [InheritedExport]
    public interface IParser
    {
        double Priority { get; }

        bool CanParse([NotNull] IParseContext context);

        void Parse([NotNull] IParseContext context);
    }
}
