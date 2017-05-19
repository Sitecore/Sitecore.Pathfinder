// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    public enum MergingMatch
    {
        MatchUsingItemPath,

        MatchUsingSourceFile
    }

    public class Item : DatabaseProjectItem
    {
        [NotNull]
        public static readonly Item Empty = new Item(Database.Empty, new Guid("{935B8D6C-D25A-48B8-8167-2C0443D77027}"), string.Empty, string.Empty, string.Empty);

        [CanBeNull]
        private ItemAppearance _appearance;

        [CanBeNull, ItemNotNull]
        private ChildrenCollection _children;

        [CanBeNull, ItemNotNull]
        private FieldCollection _fields;

        [CanBeNull]
        private ItemHelp _help;

        [CanBeNull]
        private string _parentPath;

        [CanBeNull]
        private ItemPath _paths;

        [CanBeNull]
        private ItemPublishing _publishing;

        [CanBeNull]
        private ItemStatistics _statistics;

        public Item([NotNull] Database database, Guid guid, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath) : base(database, guid, itemName, itemIdOrPath)
        {
            TemplateIdOrPathProperty = NewSourceProperty("Template", string.Empty, SourcePropertyFlags.IsQualified);
            TemplateIdOrPath = templateIdOrPath;
            SortorderProperty = NewSourceProperty("Sortorder", 0);
        }

        [NotNull]
        public ItemAppearance Appearance => _appearance ?? (_appearance = new ItemAppearance(this));

        [NotNull, ItemNotNull]
        public ChildrenCollection Children => _children ?? (_children = new ChildrenCollection(this));

        [NotNull, ItemNotNull]
        public FieldCollection Fields => _fields ?? (_fields = new FieldCollection(this));

        [NotNull]
        public ItemHelp Help => _help ?? (_help = new ItemHelp(this));

        [NotNull]
        public string this[[NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => Fields.GetFieldValue(fieldName, language, version);

        [NotNull]
        public string this[Guid fieldId, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => Fields.GetFieldValue(fieldId, language, version);

        public MergingMatch MergingMatch { get; set; }

        public bool OverwriteWhenMerging { get; set; }

        [CanBeNull, Obsolete("Use GetParent() instead", false)]
        public Item Parent => GetParent();

        [NotNull]
        public string ParentItemPath => _parentPath ?? (_parentPath = PathHelper.GetItemParentPath(ItemIdOrPath));

        [NotNull]
        public ItemPath Paths => _paths ?? (_paths = new ItemPath(this));

        [NotNull]
        public ItemPublishing Publishing => _publishing ?? (_publishing = new ItemPublishing(this));

        public int Sortorder
        {
            get => SortorderProperty.GetValue();
            set => SortorderProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<int> SortorderProperty { get; }

        [NotNull]
        public ItemStatistics Statistics => _statistics ?? (_statistics = new ItemStatistics(this));

        [NotNull]
        public Template Template
        {
            get
            {
                var templateIdOrPath = TemplateIdOrPath;

                if (templateIdOrPath.Contains('/') || templateIdOrPath.Contains('{'))
                {
                    return Database.FindQualifiedItem<Template>(templateIdOrPath) ?? Template.Empty;
                }

                // resolve by short name
                var templates = Database.GetByShortName<Template>(templateIdOrPath).ToArray();
                return templates.Length == 1 ? templates.First() : Template.Empty;
            }
        }

        [NotNull, Obsolete("Use Template.Uri.Guid instead")]
        public ID TemplateID => Template.ID;

        [NotNull]
        public string TemplateIdOrPath
        {
            get => TemplateIdOrPathProperty.GetValue();
            set => TemplateIdOrPathProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> TemplateIdOrPathProperty { get; }

        [NotNull]
        public string TemplateName => Template.ItemName;

        [NotNull]
        public string GetDisplayName([NotNull] Language language, [NotNull] Version version)
        {
            var displayName = Fields.GetFieldValue("__Display Name", language, version);
            return string.IsNullOrEmpty(displayName) ? ItemName : displayName;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Language> GetLanguages()
        {
            return Fields.Where(f => f.Language != Language.Undefined && f.Language != Language.Empty).Select(f => f.Language).Distinct();
        }

        [CanBeNull]
        public Item GetParent()
        {
            return Database.FindQualifiedItem<Item>(ParentItemPath);
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Version> GetVersions([NotNull] Language language)
        {
            return Fields.Where(f => f.Language == language && f.Version != Version.Undefined).Select(f => f.Version).Distinct();
        }

        public void Merge([NotNull] Item newProjectItem)
        {
            Merge(newProjectItem, OverwriteWhenMerging);
        }

        [NotNull]
        public Item With([NotNull] ITextNode textNode)
        {
            AddSourceTextNode(textNode);
            return this;
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newItem = newProjectItem as Item;
            Assert.Cast(newItem, nameof(newItem));

            if (!string.IsNullOrEmpty(newItem.TemplateIdOrPath))
            {
                TemplateIdOrPathProperty.SetValue(newItem.TemplateIdOrPathProperty);
            }

            if (newItem.Sortorder != 0)
            {
                SortorderProperty.SetValue(newItem.SortorderProperty);
            }

            OverwriteWhenMerging = OverwriteWhenMerging && newItem.OverwriteWhenMerging;
            MergingMatch = MergingMatch == MergingMatch.MatchUsingSourceFile && newItem.MergingMatch == MergingMatch.MatchUsingSourceFile ? MergingMatch.MatchUsingSourceFile : MergingMatch.MatchUsingItemPath;

            foreach (var newField in newItem.Fields)
            {
                var field = Fields.FirstOrDefault(f => string.Equals(f.FieldName, newField.FieldName, StringComparison.OrdinalIgnoreCase) && f.Language == newField.Language && f.Version == newField.Version);
                if (field == null)
                {
                    newField.Item = this;
                    Fields.Add(newField);
                    continue;
                }

                /*
                // todo: enable this check
                if (field.Value != newField.Value)
                {
                    context.Trace.TraceError(Texts.Field_is_being_assigned_two_different_values, field.FieldName);
                }
                */

                field.ValueProperty.SetValue(newField.ValueProperty);
            }
        }
    }
}
