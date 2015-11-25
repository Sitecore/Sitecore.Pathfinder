// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlLanguage : LanguageBase
    {
        public override bool CanHandleExtension(string extension)
        {
            return string.Equals(extension, "item.xml", StringComparison.OrdinalIgnoreCase);
        }

        public override void WriteItem(TextWriter textWriter, Item item)
        {
            item.WriteAsXml(textWriter);
        }

        public override void WriteLayout(TextWriter textWriter, string databaseName, LayoutBuilder layoutBuilder)
        {
            layoutBuilder.WriteAsXml(textWriter, databaseName);
        }

        public override void WriteTemplate(TextWriter textWriter, Template template)
        {
            template.WriteAsXml(textWriter);
        }
    }
}
