using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class JsonTemplateTextNodeParser : TemplateTextNodeParserBase
    {
        [ImportingConstructor]
        public JsonTemplateTextNodeParser([NotNull] ISchemaService schemaService) : base(schemaService, Constants.TextNodeParsers.Templates)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is JsonTextSnapshot && textNode.Key == "Template";
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return !string.IsNullOrEmpty(textNode.Value) ? textNode : base.GetItemNameTextNode(context, textNode, attributeName);
        }
    }
}