// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    [Serializable]
    public class XPathException : Exception
    {
        public XPathException()
        {
        }

        public XPathException([NotNull] string message) : base(message)
        {
        }

        public XPathException([NotNull] string message, [NotNull] Exception innerException) : base(message, innerException)
        {
        }
    }
}
