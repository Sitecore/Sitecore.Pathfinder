// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
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

    public class Item : ItemBase, IXPathItem
    {
        [NotNull]
        public static readonly Item Empty = new Item(Projects.Project.Empty, TextNode.Empty, new Guid("{935B8D6C-D25A-48B8-8167-2C0443D77027}"), "emptydatabase", string.Empty, string.Empty, string.Empty);

        [CanBeNull]
        [ItemNotNull]
        private ChildrenCollection _children;

        [CanBeNull]
        [ItemNotNull]
        private FieldCollection _fields;

        [CanBeNull]
        private ItemPublishing _publishing;

        [CanBeNull]
        private Template _template;

        public Item([NotNull] IProject project, [NotNull] ITextNode textNode, Guid guid, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath) : base(project, textNode, guid, databaseName, itemName, itemIdOrPath)
        {
            TemplateIdOrPath = templateIdOrPath;
        }

        [NotNull]
        [ItemNotNull]
        public ChildrenCollection Children => _children ?? (_children = new ChildrenCollection(this));

        [NotNull]
        [ItemNotNull]
        public FieldCollection Fields => _fields ?? (_fields = new FieldCollection(this));

        [NotNull]
        public string this[string fieldName]
        {
            get
            {
                // todo: handle languages and versions
                Field field;
                if (fieldName.StartsWith("{") && fieldName.StartsWith("}"))
                {
                    Guid guid;
                    if (Guid.TryParse(fieldName, out guid))
                    {
                        field = Fields.FirstOrDefault(f => f.FieldId == guid);
                        return field?.Value ?? string.Empty;
                    }
                }

                field = Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                return field?.Value ?? string.Empty;
            }
        }

        [NotNull]
        public string this[Guid guid]
        {
            get
            {
                // todo: handle languages and versions
                var field = Fields.FirstOrDefault(f => f.FieldId == guid);
                return field?.Value ?? string.Empty;
            }
        }

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

        [Obsolete("Use GetParent() instead", false)]
        [CanBeNull]
        public Item Parent => GetParent();

        [NotNull]
        public ItemPublishing Publishing => _publishing ?? (_publishing = new ItemPublishing(this));

        [NotNull]
        public Template Template
        {
            get
            {
                if (_template == null || _template == Template.Empty)
                {
                    var templateIdOrPath = TemplateIdOrPath;

                    if (templateIdOrPath.Contains('/') || templateIdOrPath.Contains('{'))
                    {
                        _template = Project.FindQualifiedItem(DatabaseName, templateIdOrPath) as Template;
                    }
                    else
                    {
                        // resolve by short name
                        var templates = Project.ProjectItems.OfType<Template>().Where(t => t.ShortName == templateIdOrPath && string.Equals(t.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
                        _template = templates.Count == 1 ? templates.First() : null;
                    }
                }

                return _template ?? Template.Empty;
            }
        }

        [NotNull]
        [Obsolete("Use Template.Uri.Guid instead")]
        public ID TemplateID => Template.ID;

        [NotNull]
        public string TemplateIdOrPath
        {
            get { return TemplateIdOrPathProperty.GetValue(); }
            set
            {
                TemplateIdOrPathProperty.SetValue(value);
                _template = null;
            }
        }

        [NotNull]
        public SourceProperty<string> TemplateIdOrPathProperty { get; } = new SourceProperty<string>("Template", string.Empty, SourcePropertyFlags.IsQualified);

        public string TemplateName => Template.ItemName;

        string IXPathItem.ItemId => Uri.Guid.Format();

        string IXPathItem.TemplateId => Template.Uri.Guid.Format();

        [NotNull]
        [ItemNotNull]
        public virtual IEnumerable<Item> GetChildren()
        {
            var itemIdOrPath = ItemIdOrPath + "/";
            var index = itemIdOrPath.Length;

            return Project.Items.Where(i => i.ItemIdOrPath.StartsWith(itemIdOrPath, StringComparison.OrdinalIgnoreCase) && i.ItemIdOrPath.IndexOf('/', index) < 0);
        }

        [NotNull]
        public string GetDisplayName([NotNull] string language, int version)
        {
            var displayName = Fields.GetFieldValue("__Display Name", language, version);
            return string.IsNullOrEmpty(displayName) ? ItemName : displayName;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<string> GetLanguages()
        {
            return Fields.Where(f => !string.IsNullOrEmpty(f.Language)).Select(f => f.Language).Distinct();
        }

        [CanBeNull]
        public Item GetParent()
        {
            var n = ItemIdOrPath.LastIndexOf('/');
            if (n <= 0)
            {
                return null;
            }

            var parentItemPath = ItemIdOrPath.Left(n);
            return Project.FindQualifiedItem(parentItemPath) as Item;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<int> GetVersions([NotNull] string language)
        {
            return Fields.Where(f => string.Equals(f.Language, language, StringComparison.OrdinalIgnoreCase) && f.Version != 0).Select(f => f.Version).Distinct();
        }

        public void Merge([NotNull] Item newProjectItem)
        {
            Merge(newProjectItem, OverwriteWhenMerging);
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

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
                var field = Fields.FirstOrDefault(f => string.Equals(f.FieldName, newField.FieldName, StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, newField.Language, StringComparison.OrdinalIgnoreCase) && f.Version == newField.Version);
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

                field.ValueProperty.SetValue(newField.ValueProperty, SetValueOptions.DisableUpdates);
            }
        }

        protected override void OnProjectChanged(object sender)
        {
            _template = null;
        }

        IEnumerable<IXPathItem> IXPathItem.GetChildren()
        {
            foreach (var child in GetChildren())
            {
                yield return child;
            }

            // todo: return XPath items
        }

        IXPathItem IXPathItem.GetParent()
        {
            var parent = GetParent();
            if (parent != null)
            {
                return parent;
            }

            var n = ItemIdOrPath.LastIndexOf('/');
            return n > 0 ? new XPathItem(Project, ItemIdOrPath.Left(n)) : null;
        }
    }
}
