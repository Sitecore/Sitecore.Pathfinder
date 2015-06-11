// © 2015 Sitecore Corporation A/S. All rights reserved.
// ReSharper disable CodeAnnotationAnalyzer
// ReSharper disable InconsistentNaming

using System.Diagnostics;

namespace Sitecore.Pathfinder.Diagnostics
{
    using System.Xml;

    public static class SourceTemplates
    {
        [SourceTemplate]
        [Conditional("SourceTemplates")]
        public static void attr(this XmlTextWriter writer, string name, string value)
        {        
            writer.WriteAttributeString(name, value);
        }
    }
}