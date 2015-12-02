// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class ParseException : Exception
    {
        public ParseException([NotNull] string message) : base(message)
        {
        }
    }
}
