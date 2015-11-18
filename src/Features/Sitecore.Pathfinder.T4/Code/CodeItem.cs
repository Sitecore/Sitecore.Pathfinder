// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeItem
    {
        [CanBeNull]
        [ItemNotNull]
        private CodeItemChildCollection _children;

        [CanBeNull]
        [ItemNotNull]
        private CodeItemFieldCollection _fields;

        [CanBeNull]
        private ID _id;

        [CanBeNull]
        private CodeTemplate _template;

        public CodeItem([NotNull] CodeProject project, [NotNull] Item innerItem)
        {
            Project = project;
            InnerItem = innerItem;
        }

        [NotNull]
        [ItemNotNull]
        public CodeItemChildCollection Children => _children ?? (_children = new CodeItemChildCollection(this));

        [NotNull]
        public CodeDatabase Database => Project.GetDatabase(InnerItem.DatabaseName);

        [NotNull]
        [ItemNotNull]
        public CodeItemFieldCollection Fields => _fields ?? (_fields = new CodeItemFieldCollection(this));

        [NotNull]
        public ID ID => _id ?? (_id = new ID(InnerItem.Uri.Guid));

        [NotNull]
        public Item InnerItem { get; set; }

        [NotNull]
        public string this[[NotNull] string fieldName]
        {
            get
            {
                var field = InnerItem.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                return field?.Value ?? string.Empty;
            }
        }

        [NotNull]
        public string Name => InnerItem.ItemName;

        [NotNull]
        public CodeProject Project { get; }

        [NotNull]
        public CodeTemplate Template => _template ?? (_template = new CodeTemplate(Project, InnerItem.Template));

        [NotNull]
        public ID TemplateID => Template.ID;

        [NotNull]
        public string TemplateName => Template.Name;

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((CodeTemplate)obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator ==([CanBeNull] CodeItem left, [CanBeNull] CodeItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] CodeItem left, [CanBeNull] CodeItem right)
        {
            return !Equals(left, right);
        }

        protected bool Equals([NotNull] CodeTemplate other)
        {
            return Equals(ID, other.ID);
        }
    }
}
