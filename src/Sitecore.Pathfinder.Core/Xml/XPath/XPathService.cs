// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Functions;

namespace Sitecore.Pathfinder.Xml.XPath
{
    [Export(typeof(IXPathService))]
    public class XPathService : IXPathService
    {
        [ImportingConstructor]
        public XPathService([ImportMany, ItemNotNull, NotNull]   IEnumerable<IFunction> functions)
        {
            Functions = functions;
        }

        public IEnumerable<IFunction> Functions { get; }

        public virtual XPathExpression GetXPathExpression()
        {
            return new XPathExpression(this);
        }

        public XPathExpression GetXPathExpression(string xpath)
        {
            return GetXPathExpression().Parse(xpath);
        }
    }
}
