// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonLanguage : LanguageBase
    {
        public override bool CanHandleExtension(string extension)
        {
            return string.Equals(extension, "item.json", StringComparison.OrdinalIgnoreCase);
        }

        public override void WriteItem(TextWriter textWriter, Item item)
        {
            item.WriteAsJson(textWriter);
        }

        public override void WriteLayout(TextWriter textWriter, string databaseName, LayoutBuilder layoutBuilder)
        {
            layoutBuilder.WriteAsJson(textWriter);
        }

        public override void WriteTemplate(TextWriter textWriter, Template template)
        {
            template.WriteAsJson(textWriter);
        }
    }
}
