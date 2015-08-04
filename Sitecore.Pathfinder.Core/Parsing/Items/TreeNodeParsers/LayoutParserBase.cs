// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class LayoutParserBase : TextNodeParserBase
    {
        protected LayoutParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemName = GetItemName(context.ParseContext, textNode);
            var itemIdOrPath = context.ParentItemPath + "/" + itemName.Value;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName.Value, itemIdOrPath, string.Empty);
            item.ItemName.Merge(itemName);

            var field = context.ParseContext.Factory.Field(item, "__Renderings", string.Empty);
            field.Value.AddSource(textNode.GetInnerTextNode());
            field.ValueHint.SetValue("Layout");

            item.Fields.Add(field);

            item.References.AddRange(ParseReferences(context, item, textNode, string.Empty));

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected virtual void ParseDeviceReferences([NotNull] ItemParseContext context, [NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
        {
            var deviceNameAttribute = new Attribute<string>("Name", string.Empty);
            deviceNameAttribute.SourceFlags = SourceFlags.IsShort;
            deviceNameAttribute.Parse(deviceTextNode);

            references.Add(context.ParseContext.Factory.DeviceReference(projectItem, deviceNameAttribute, deviceNameAttribute.Value));

            var layoutAttribute = new Attribute<string>("Layout", string.Empty);
            layoutAttribute.SourceFlags = SourceFlags.IsShort;
            layoutAttribute.Parse(deviceTextNode);

            references.Add(context.ParseContext.Factory.LayoutReference(projectItem, layoutAttribute, layoutAttribute.Value));

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
                var textNode = new Attribute<string>(renderingTextNode.Name, string.Empty);
                textNode.AddSource(new AttributeNameTextNode(renderingTextNode));
                textNode.SourceFlags = SourceFlags.IsShort;

                references.Add(context.ParseContext.Factory.LayoutRenderingReference(projectItem, textNode, renderingTextNode.Name));
            }

            foreach (var childTextNode in renderingTextNode.ChildNodes)
            {
                ParseRenderingReferences(context, references, projectItem, childTextNode);
            }
        }
    }
}

