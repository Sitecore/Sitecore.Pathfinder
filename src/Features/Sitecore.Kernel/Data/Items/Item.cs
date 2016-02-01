// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Globalization;

namespace Sitecore.Data.Items
{
    public class Item
    {
        [NotNull]
        public ItemAppearance Appearance { get; private set; }

        [NotNull, ItemNotNull]
        public IEnumerable<Item> Children { get; private set; }

        [NotNull]
        public Database Database { get; set; }

        [NotNull, ItemNotNull]
        public IEnumerable<Field> Fields { get; private set; }

        [NotNull]
        public ItemHelp Help { get; private set; }
        public ItemEditing Editing { get; private set; }

        public ID ID { get; private set; }

        [NotNull]
        public string this[[NotNull] string fieldName]
        {
            get { return string.Empty; }
            set { }
        }

        [NotNull]
        public string this[[NotNull] ID fieldId]
        {
            get { return string.Empty; }
            set { }
        }

        [NotNull]
        public Language Language { get; set; }

        [NotNull]
        public string Name { get; set; }

        [CanBeNull]
        public Item Parent { get; private set; }

        [NotNull]
        public ItemPaths Paths { get; private set; }

        [NotNull]
        public TemplateItem Template { get; private set; }

        [NotNull]
        public ID TemplateID { get; private set; }

        [NotNull]
        public string TemplateName { get; private set; }

        [NotNull]
        public Version Version { get; set; }

        [CanBeNull]
        public ID ParentID { get; private set; }

        public void ChangeTemplate(TemplateItem templateItem)
        {
        }

        public void Delete()
        {
        }

        public void MoveTo([NotNull] Item parentItem)
        {
        }

        public void Recycle()
        {
        }
    }
}
