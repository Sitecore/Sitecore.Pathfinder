// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class XmContentTextNodeParser : ContentTextNodeParserBase
    {
        public XmContentTextNodeParser() : base(Constants.TextNodeParsers.Content)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is XmlTextSnapshot;
        }

        protected override void ParseUnversionedTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            foreach (var languageChildNode in textNode.ChildNodes)
            {
                var languageVersionContext = new LanguageVersionContext();
                languageVersionContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));

                ParseAttributes(context, item, languageVersionContext, languageChildNode);
                ParseFields(context, item, languageVersionContext, languageChildNode);

                foreach (var childNode in languageChildNode.ChildNodes.Where(c => c.Key == "Version"))
                {
                    var versionVersionContext = new LanguageVersionContext();
                    versionVersionContext.LanguageProperty.SetValue(languageVersionContext.LanguageProperty);
                    versionVersionContext.VersionProperty.Parse(childNode);

                    ParseAttributes(context, item, versionVersionContext, childNode);
                    ParseFields(context, item, languageVersionContext, childNode);
                }
            }
        }

        protected void ParseFields([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode parentTextNode)
        {
            foreach (var childNode in parentTextNode.ChildNodes.Where(c => c.Key != "Version"))
            {
                ParseFieldTextNode(context, item, languageVersionContext, childNode);
            }
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode textNode)
        {
            var childNode = textNode.ChildNodes.FirstOrDefault();
            if (childNode == null)
            {
                return;
            }

            var parser = new XmlLayoutTextNodeParser();
            parser.Parse(context, childNode, item);
        }
    }
}
