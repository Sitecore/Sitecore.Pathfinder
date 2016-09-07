// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Projects.Items
{
    public enum MergingMatch
    {
        MatchUsingItemPath,

        MatchUsingSourceFile
    }

    public class Item : DatabaseProjectItem, IXPathItem
    {
        [NotNull]
        public static readonly Item Empty = new Item(Projects.Project.Empty, new Guid("{935B8D6C-D25A-48B8-8167-2C0443D77027}"), "emptydatabase", string.Empty, string.Empty, string.Empty);

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

        public Item([NotNull] IProjectBase project, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath) : base(project, guid, databaseName, itemName, itemIdOrPath)
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

        public string this[string fieldName] => Fields.GetFieldValue(fieldName);

        [NotNull]
        public string this[Guid guid] => Fields.GetFieldValue(guid);

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
                    return Project.FindQualifiedItem<Template>(Database, templateIdOrPath) ?? Template.Empty;
                }

                // resolve by short name
                var templates = Project.GetByShortName<Template>(Database, templateIdOrPath).ToArray();
                return templates.Length == 1 ? templates.First() : Template.Empty;
            }
        }

        [NotNull, Obsolete("Use Template.Uri.Guid instead")]
        public ID TemplateID => Template.ID;

        [NotNull]
        public string TemplateIdOrPath
        {
            get { return TemplateIdOrPathProperty.GetValue(); }
            set { TemplateIdOrPathProperty.SetValue(value); }
        }

        public int Sortorder
        {
            get { return SortorderProperty.GetValue(); }
            set { SortorderProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> TemplateIdOrPathProperty { get; }

        [NotNull]
        public SourceProperty<int> SortorderProperty { get; }

        public string TemplateName => Template.ItemName;

        string IXPathItem.ItemId => Uri.Guid.Format();

        string IXPathItem.ItemPath => ItemIdOrPath;

        string IXPathItem.TemplateId => Template.Uri.Guid.Format();

        [NotNull, ItemNotNull]
        public virtual IEnumerable<Item> GetChildren()
        {
            return Project.GetChildren(this);
        }

        [NotNull]
        public string GetDisplayName([NotNull] Language language, [NotNull] Version version)
        {
            var displayName = Fields.GetFieldValue("__Display Name", language, version);
            return string.IsNullOrEmpty(displayName) ? ItemName : displayName;
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Language> GetLanguages()
        {
            return Fields.Where(f => f.Language != Language.Undefined).Select(f => f.Language).Distinct();
        }

        [CanBeNull]
        public Item GetParent()
        {
            return Project.FindQualifiedItem<Item>(Database, ParentItemPath);
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

        IEnumerable<IXPathItem> IXPathItem.GetChildren()
        {
            var childNames = new HashSet<string>();

            foreach (var child in GetChildren())
            {
                yield return child;
                childNames.Add(child.ItemName);
            }

            // yield virtual paths that are used by items deeper in the hierachy - tricky, tricky
            var itemIdOrPath = ItemIdOrPath + "/";
            foreach (var descendent in Database.GetItems().Where(i => i.ItemIdOrPath.StartsWith(itemIdOrPath, StringComparison.OrdinalIgnoreCase)))
            {
                var n = descendent.ItemIdOrPath.IndexOf('/', itemIdOrPath.Length);
                if (n < 0)
                {
                    continue;
                }

                var childName = descendent.ItemIdOrPath.Mid(itemIdOrPath.Length, n - itemIdOrPath.Length);
                if (childNames.Contains(childName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return new XPathItem(Project, DatabaseName, itemIdOrPath + childName);
                childNames.Add(childName);
            }
        }

        IXPathItem IXPathItem.GetParent()
        {
            if (string.IsNullOrEmpty(ParentItemPath))
            {
                return null;
            }

            var parent = GetParent();
            if (parent != null)
            {
                return parent;
            }

            return new XPathItem(Project, DatabaseName, ParentItemPath);
        }
    }
}
