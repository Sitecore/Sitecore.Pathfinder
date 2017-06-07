// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class LayoutTextNodeParserBase : TextNodeParserBase
    {
        [NotNull]
        protected IReferenceParserService ReferenceParserService { get; }

        protected LayoutTextNodeParserBase([NotNull] IReferenceParserService referenceParserService, double priority) : base(priority)
        {
            ReferenceParserService = referenceParserService;
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var itemIdOrPath = PathHelper.CombineItemPath(context.ParentItemPath, itemNameTextNode.Value);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.Database.DatabaseName);
            var database = context.ParseContext.Project.GetDatabase(databaseName);

            var item = context.ParseContext.Factory.Item(database, guid, itemNameTextNode.Value, itemIdOrPath, string.Empty).With(textNode);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);

            Parse(context, textNode, item);

            context.ParseContext.Project.AddOrMerge(item);
        }

        public virtual void Parse([NotNull] ItemParseContext context, [NotNull] ITextNode textNode, [NotNull] Item item)
        {
            var field = context.ParseContext.Factory.Field(item, "__Renderings", string.Empty).With(textNode);

            // todo: set template field

            var innerTextNode = textNode.Inner;
            if (innerTextNode != null)
            {
                // use the inner value but the outer text node
                field.ValueProperty.SetValue(innerTextNode.Value);
            }

            field.ValueProperty.AddSourceTextNode(textNode);

            item.Fields.Add(field);
        }

        protected virtual void ParseDeviceReferences([NotNull] ItemParseContext context, [NotNull, ItemNotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
        {
            var deviceNameProperty = new SourceProperty<string>(projectItem, "Name", string.Empty, SourcePropertyFlags.IsShort);
            deviceNameProperty.Parse(deviceTextNode);
            references.Add(context.ParseContext.Factory.DeviceReference(projectItem, deviceNameProperty, string.Empty, context.ParseContext.Database.DatabaseName));

            var layoutProperty = new SourceProperty<string>(projectItem, "Layout", string.Empty, SourcePropertyFlags.IsShort);
            layoutProperty.Parse(deviceTextNode);
            references.Add(context.ParseContext.Factory.LayoutReference(projectItem, layoutProperty, string.Empty, context.ParseContext.Database.DatabaseName));

            foreach (var renderingTextNode in deviceTextNode.ChildNodes)
            {
                ParseRenderingReferences(context, references, projectItem, renderingTextNode);
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<IReference> ParseReferences([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode layoutTextNode)
        {
            var result = ReferenceParserService.ParseReferences(projectItem, layoutTextNode).ToList();

            foreach (var deviceTextNode in layoutTextNode.ChildNodes)
            {
                ParseDeviceReferences(context, result, projectItem, deviceTextNode);
            }

            return result;
        }

        protected virtual void ParseRenderingReferences([NotNull] ItemParseContext context, [NotNull, ItemNotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode)
        {
            if (!string.IsNullOrEmpty(renderingTextNode.Key))
            {
                var sourceProperty = new SourceProperty<string>(projectItem, renderingTextNode.Key, string.Empty, SourcePropertyFlags.IsShort);
                sourceProperty.SetValue(new AttributeNameTextNode(renderingTextNode));

                references.Add(context.ParseContext.Factory.LayoutRenderingReference(projectItem, sourceProperty, string.Empty, context.ParseContext.Database.DatabaseName));
            }

            // parse references for rendering properties
            foreach (var attributeTextNode in renderingTextNode.Attributes)
            {
                foreach (var reference in ReferenceParserService.ParseReferences(projectItem, attributeTextNode))
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
