// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Collections;
using Sitecore.Globalization;

namespace Sitecore.Data.Items
{
    public class Item
    {
        [NotNull]
        public ItemAppearance Appearance
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ChildList Children
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Database Database
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public ItemEditing Editing
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public FieldCollection Fields
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ItemHelp Help
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID ID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string this[[NotNull] string fieldName]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public string this[[NotNull] ID fieldId]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public Language Language
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [NotNull]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Item Parent
        {
            get { throw new NotImplementedException(); }
        }

        [CanBeNull]
        public ID ParentID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ItemPath Paths
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ItemVersions Versions
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public TemplateItem Template
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public ID TemplateID
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public string TemplateName
        {
            get { throw new NotImplementedException(); }
        }

        [NotNull]
        public Version Version
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void ChangeTemplate([NotNull] TemplateItem templateItem)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void MoveTo([NotNull] Item parentItem)
        {
            throw new NotImplementedException();
        }

        public Guid Recycle()
        {
            throw new NotImplementedException();
        }
    }
}
