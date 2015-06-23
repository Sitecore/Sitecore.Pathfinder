// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class LayoutParserBase : TextNodeParserBase
    {
        protected LayoutParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = textNode.GetAttributeTextNode("Name");
            var itemName = itemNameTextNode?.Value ?? context.ParseContext.ItemName;
            var itemIdOrPath = context.ParentItemPath + "/" + itemName;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath, string.Empty);
            item.ItemName.Source = itemNameTextNode;

            var field = context.ParseContext.Factory.Field(item, "__Renderings", string.Empty, 0, "Layout: " + textNode.Value);
            field.IsTestable = false;
            field.Value.Source = textNode;

            item.Fields.Add(field);

            item.References.AddRange(ParseReferences(context, item, textNode, string.Empty));

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected virtual void ParseDeviceReferences([NotNull] ItemParseContext context, [NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
        {
            var deviceNameTextNode = deviceTextNode.GetAttributeTextNode("Name") ?? deviceTextNode;
            references.Add(context.ParseContext.Factory.DeviceReference(projectItem, new Attribute<string>(deviceNameTextNode, SourceFlags.IsShort), deviceTextNode.GetAttributeValue("Name")));

            var layoutTextNode = deviceTextNode.GetAttributeTextNode("Layout");
            if (layoutTextNode != null)
            {
                references.Add(context.ParseContext.Factory.LayoutReference(projectItem, new Attribute<string>(layoutTextNode, SourceFlags.IsShort), deviceTextNode.GetAttributeValue("Layout")));
            }

            foreach (var renderingsTextNode in deviceTextNode.ChildNodes)
            {
                foreach (var renderingTextNode in renderingsTextNode.ChildNodes)
                {
                  ParseRenderingReferences(context, references, projectItem, renderingTextNode);
                }
            }
        }

        protected override IEnumerable<IReference> ParseReferences(ItemParseContext context, IProjectItem projectItem, ITextNode source, string text)
        {
            var result = base.ParseReferences(context, projectItem, source, text).ToList();

            var layoutTextNode = source.ChildNodes.FirstOrDefault();
            if (layoutTextNode == null)
            {
                return result;
            }

            foreach (var deviceTextNode in layoutTextNode.ChildNodes)
            {
                ParseDeviceReferences(context, result, projectItem, deviceTextNode);
            }

            return result;
        }

        protected virtual void ParseRenderingReferences([NotNull] ItemParseContext context, [NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode)
        {
            if (!string.IsNullOrEmpty(renderingTextNode.Name))
            {
                references.Add(context.ParseContext.Factory.LayoutRenderingReference(projectItem, new Attribute<string>(new AttributeNameTextNode(renderingTextNode), SourceFlags.IsShort), renderingTextNode.Name));
            }

            foreach (var childTextNode in renderingTextNode.ChildNodes)
            {
                ParseRenderingReferences(context, references, projectItem, childTextNode);
            }
        }
    }
}
