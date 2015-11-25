// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlLanguage : LanguageBase
    {
        public override bool CanHandleExtension(string extension)
        {
            return string.Equals(extension, "item.yaml", StringComparison.OrdinalIgnoreCase);
        }

        public override void WriteItem(TextWriter textWriter, Item item)
        {
            item.WriteAsYaml(textWriter);
        }

        public override void WriteLayout(TextWriter textWriter, string databaseName, LayoutBuilder layoutBuilder)
        {
            layoutBuilder.WriteAsYaml(textWriter);
        }

        public override void WriteTemplate(TextWriter textWriter, Template template)
        {
            template.WriteAsYaml(textWriter);
        }
    }
}
