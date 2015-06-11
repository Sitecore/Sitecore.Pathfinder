// © 2015 Sitecore Corporation A/S. All rights reserved.
namespace Sitecore.Pathfinder.Diagnostics
{
    using System.Xml;

    public static class SourceTemplates
    {
        [SourceTemplate]
        public static void attr(this XmlTextWriter writer, string name, string value)
        {        
            writer.WriteAttributeString(name, value);
        }
    }
}