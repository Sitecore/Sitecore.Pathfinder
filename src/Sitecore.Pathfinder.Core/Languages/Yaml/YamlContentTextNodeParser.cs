// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class YamlContentTextNodeParser : ContentTextNodeParserBase
    {
        public YamlContentTextNodeParser() : base(Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot && textNode.Snapshot.SourceFile.AbsoluteFileName.EndsWith(".content.yaml", StringComparison.OrdinalIgnoreCase);
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return string.IsNullOrEmpty(textNode.Value) ? base.GetItemNameTextNode(context, textNode, attributeName) : textNode;
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var parser = new YamlLayoutTextNodeParser();
            parser.Parse(context, textNode, item);
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode childNode)
        {
            foreach (var languageChildNode in childNode.ChildNodes)
            {
                var languageVersionContext = new LanguageVersionContext();
                languageVersionContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));

                ParseAttributes(context, item, languageVersionContext, languageChildNode);

                foreach (var versionChildNode in languageChildNode.ChildNodes)
                {
                    var versionVersionContext = new LanguageVersionContext();
                    versionVersionContext.LanguageProperty.SetValue(languageVersionContext.LanguageProperty);
                    versionVersionContext.VersionProperty.SetValue(new AttributeNameTextNode(versionChildNode));

                    ParseAttributes(context, item, versionVersionContext, versionChildNode);
                }
            }
        }
    }
}
