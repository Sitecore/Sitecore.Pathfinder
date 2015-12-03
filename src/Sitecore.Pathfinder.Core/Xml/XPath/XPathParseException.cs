// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class XPathParseException : Exception
    {
        public XPathParseException([NotNull] string message) : base(message)
        {
        }
    }
}
