// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class XmContentTextNodeParser : ContentTextNodeParserBase
    {
        [ImportingConstructor]
        public XmContentTextNodeParser([NotNull] IFactory factory, [NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser, [NotNull] ISchemaService schemaService) : base(factory, trace, referenceParser, schemaService, Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode) => textNode.Snapshot is XmlTextSnapshot;

        protected void ParseFields([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode parentTextNode)
        {
            foreach (var childNode in parentTextNode.ChildNodes.Where(c => c.Key != "Version"))
            {
                ParseFieldTextNode(context, item, languageVersionContext, childNode);
            }
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode fieldsTextNode)
        {
            // parse shared fields
            var fieldContext = new LanguageVersionContext();
            ParseAttributes(context, item, fieldContext, fieldsTextNode);

            // parse unversioned and versioned fields
            foreach (var languageChildNode in fieldsTextNode.ChildNodes)
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

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode layoutTextNode)
        {
            var childNode = layoutTextNode.ChildNodes.FirstOrDefault();
            if (childNode == null)
            {
                return;
            }

            var parser = new XmlLayoutTextNodeParser(Factory, ReferenceParser);
            parser.Parse(context, childNode, item);
        }
    }
}
