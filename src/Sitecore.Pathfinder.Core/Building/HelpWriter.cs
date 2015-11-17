// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    public class HelpWriter
    {
        public HelpWriter()
        {
            Remarks = new StringWriter();
            Summary = new StringWriter();
            Parameters = new StringWriter();
            Examples = new StringWriter();
        }

        [NotNull]
        public TextWriter Examples { get; }

        [NotNull]
        public TextWriter Parameters { get; }

        [NotNull]
        public TextWriter Remarks { get; }

        [NotNull]
        public TextWriter Summary { get; }

        [NotNull]
        public string GetExamples()
        {
            return Examples.ToString();
        }

        [NotNull]
        public string GetParameters()
        {
            var result = Parameters.ToString();
            if (string.IsNullOrEmpty(result))
            {
                result = "None";
            }

            return result;
        }

        [NotNull]
        public string GetRemarks()
        {
            var result = Remarks.ToString();
            if (string.IsNullOrEmpty(result))
            {
                result = GetSummary();
            }

            return result;
        }

        [NotNull]
        public string GetSummary()
        {
            return Summary.ToString();
        }
    }
}
