// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    public enum MergingMatch
    {
        MatchUsingItemPath,

        MatchUsingSourceFile
    }

    public class Item : ItemBase
    {
        [NotNull]
        public static readonly Item Empty = new Item(Projects.Project.Empty, new Guid("{935B8D6C-D25A-48B8-8167-2C0443D77027}"), TextNode.Empty, "emptydatabase", string.Empty, string.Empty, string.Empty);

        public Item([NotNull] IProject project, Guid guid, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath) : base(project, guid, textNode, databaseName, itemName, itemIdOrPath)
        {
            TemplateIdOrPath = templateIdOrPath;
        }

        [NotNull]
        [ItemNotNull]
        public IList<Field> Fields { get; } = new List<Field>();

        [NotNull]
        public string LayoutHtmlFile
        {
            get { return LayoutHtmlFileProperty.GetValue(); }
            set { LayoutHtmlFileProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LayoutHtmlFileProperty { get; } = new SourceProperty<string>("Layout.HtmlFile", string.Empty);

        public MergingMatch MergingMatch { get; set; }

        public bool OverwriteWhenMerging { get; set; }

        [NotNull]
        public Template Template
        {
            get
            {
                var templateIdOrPath = TemplateIdOrPath;

                if (templateIdOrPath.Contains('/') || templateIdOrPath.Contains('{'))
                {
                    return Project.FindQualifiedItem(templateIdOrPath, DatabaseName) as Template ?? Template.Empty;
                }

                // resolve by short name
                var templates = Project.Items.OfType<Template>().Where(t => t.ShortName == templateIdOrPath && string.Equals(t.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
                return templates.Count == 1 ? templates.First() : Template.Empty;
            }
        }

        [NotNull]
        public string TemplateIdOrPath
        {
            get { return TemplateIdOrPathProperty.GetValue(); }
            set { TemplateIdOrPathProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> TemplateIdOrPathProperty { get; } = new SourceProperty<string>("Template", string.Empty, SourcePropertyFlags.IsQualified);

        public void Merge([NotNull] IParseContext context, [NotNull] Item newProjectItem)
        {
            Merge(context, newProjectItem, OverwriteWhenMerging);
        }

        protected override void Merge(IParseContext context, IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(context, newProjectItem, overwrite);

            var newItem = newProjectItem as Item;
            if (newItem == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(newItem.TemplateIdOrPath))
            {
                TemplateIdOrPathProperty.SetValue(newItem.TemplateIdOrPathProperty, SetValueOptions.DisableUpdates);
            }

            OverwriteWhenMerging = OverwriteWhenMerging && newItem.OverwriteWhenMerging;
            MergingMatch = MergingMatch == MergingMatch.MatchUsingSourceFile && newItem.MergingMatch == MergingMatch.MatchUsingSourceFile ? MergingMatch.MatchUsingSourceFile : MergingMatch.MatchUsingItemPath;

            foreach (var newField in newItem.Fields)
            {
                var field = Fields.FirstOrDefault(f => string.Compare(f.FieldName, newField.FieldName, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(f.Language, newField.Language, StringComparison.OrdinalIgnoreCase) == 0 && f.Version == newField.Version);
                if (field == null)
                {
                    newField.Item = this;
                    Fields.Add(newField);
                    continue;
                }

                if (field.Value != newField.Value)
                {
                    context.Trace.TraceError(Texts.Field_is_being_assigned_two_different_values, field.FieldName);
                }

                field.ValueProperty.SetValue(newField.ValueProperty, SetValueOptions.DisableUpdates);
                field.IsTestable = field.IsTestable || newField.IsTestable;
            }
        }
    }
}
