// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    [Serializable]
    public class QueryException : Exception
    {
        public QueryException()
        {
        }

        public QueryException([NotNull] string message) : base(message)
        {
        }

        public QueryException([NotNull] string message, [NotNull] Exception innerException) : base(message, innerException)
        {
        }
    }
}
