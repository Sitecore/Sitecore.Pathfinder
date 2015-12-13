// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class LayoutTextNodeParserBase : TextNodeParserBase
    {
        protected LayoutTextNodeParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var itemIdOrPath = PathHelper.CombineItemPath(context.ParentItemPath, itemNameTextNode.Value);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, textNode, guid, databaseName, itemNameTextNode.Value, itemIdOrPath, string.Empty);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);

            Parse(context, textNode, item);

            context.ParseContext.Project.AddOrMerge(item);
        }

        public virtual void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode, [NotNull] Item item)
        {
            var field = context.ParseContext.Factory.Field(item, textNode, "__Renderings", string.Empty);

            // todo: set template field

            var innerTextNode = textNode.GetInnerTextNode();
            if (innerTextNode != null)
            {
                // use the inner value but the outer text node
                field.ValueProperty.SetValue(innerTextNode.Value);
            }

            field.ValueProperty.AddSourceTextNode(textNode);
            field.ValueHintProperty.SetValue("Layout");

            item.Fields.Add(field);
        }

        protected virtual void ParseDeviceReferences([NotNull] ItemParseContext context, [NotNull, ItemNotNull]  ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
        {
            var deviceNameProperty = new SourceProperty<string>("Name", string.Empty, SourcePropertyFlags.IsShort);
            deviceNameProperty.Parse(deviceTextNode);
            references.Add(context.ParseContext.Factory.DeviceReference(projectItem, deviceNameProperty));

            var layoutProperty = new SourceProperty<string>("Layout", string.Empty, SourcePropertyFlags.IsShort);
            layoutProperty.Parse(deviceTextNode);
            references.Add(context.ParseContext.Factory.LayoutReference(projectItem, layoutProperty));

            var renderingsTextNode = deviceTextNode.GetSnapshotLanguageSpecificChildNode("Renderings");
            if (renderingsTextNode == null)
            {
                return;
            }

            foreach (var renderingTextNode in renderingsTextNode.ChildNodes)
            {
                ParseRenderingReferences(context, references, projectItem, renderingTextNode);
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<IReference> ParseReferences([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode layoutTextNode)
        {
            var result = context.ParseContext.ReferenceParser.ParseReferences(projectItem, layoutTextNode).ToList();

            var devicesTextNode = layoutTextNode.GetSnapshotLanguageSpecificChildNode("Devices");
            if (devicesTextNode == null)
            {
                return result;
            }

            foreach (var deviceTextNode in devicesTextNode.ChildNodes)
            {
                ParseDeviceReferences(context, result, projectItem, deviceTextNode);
            }

            return result;
        }

        protected virtual void ParseRenderingReferences([NotNull] ItemParseContext context, [NotNull, ItemNotNull]  ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode)
        {
            if (!string.IsNullOrEmpty(renderingTextNode.Key))
            {
                var sourceProperty = new SourceProperty<string>(renderingTextNode.Key, string.Empty, SourcePropertyFlags.IsShort);
                sourceProperty.SetValue(new AttributeNameTextNode(renderingTextNode));

                references.Add(context.ParseContext.Factory.LayoutRenderingReference(projectItem, sourceProperty));
            }

            // parse references for rendering properties
            foreach (var attributeTextNode in renderingTextNode.Attributes)
            {
                foreach (var reference in context.ParseContext.ReferenceParser.ParseReferences(projectItem, attributeTextNode))
                {
                    references.Add(reference);
                }
            }

            foreach (var childTextNode in renderingTextNode.ChildNodes)
            {
                ParseRenderingReferences(context, references, projectItem, childTextNode);
            }
        }
    }
}
