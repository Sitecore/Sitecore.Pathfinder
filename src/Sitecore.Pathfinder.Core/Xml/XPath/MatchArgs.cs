// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class MatchArgs : EventArgs
    {
        public MatchArgs([CanBeNull] object result)
        {
            Result = result;
        }

        public bool Abort { get; set; }

        public bool IsMatch { get; set; } = true;

        [CanBeNull]
        public object Result { get; set; }
    }
}
