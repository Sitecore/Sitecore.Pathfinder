// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Functions;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public interface IXPathService
    {
        [NotNull, ItemNotNull]
        IEnumerable<IFunction> Functions { get; }

        [NotNull]
        XPathExpression GetXPathExpression();

        [NotNull]
        XPathExpression GetXPathExpression([NotNull] string xpath);
    }
}
