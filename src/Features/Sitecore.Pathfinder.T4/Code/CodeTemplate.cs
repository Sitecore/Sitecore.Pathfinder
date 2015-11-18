// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeTemplate
    {
        [CanBeNull]
        [ItemNotNull]
        private ID[] _baseTemplates;

        [CanBeNull]
        private ID _id;

        public CodeTemplate([NotNull] CodeProject project, [NotNull] Template innerTemplate)
        {
            Project = project;
            InnerTemplate = innerTemplate;
        }

        [NotNull]
        [ItemNotNull]
        public ID[] BaseTemplates => _baseTemplates ?? (_baseTemplates = InnerTemplate.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries).Select(id => new ID(id)).ToArray());

        [NotNull]
        public CodeDatabase Database => Project.GetDatabase(InnerTemplate.DatabaseName);

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CodeTemplateField> Fields => InnerTemplate.Sections.SelectMany(s => s.Fields).Select(f => new CodeTemplateField(Project, this, f));

        [NotNull]
        public ID ID => _id ?? (_id = new ID(InnerTemplate.Uri.Guid));

        [NotNull]
        public Template InnerTemplate { get; }

        [NotNull]
        public string Name => InnerTemplate.ItemName;

        [NotNull]
        public CodeProject Project { get; }

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

        public static bool operator ==([CanBeNull] CodeTemplate left, [CanBeNull] CodeTemplate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] CodeTemplate left, [CanBeNull] CodeTemplate right)
        {
            return !Equals(left, right);
        }

        protected bool Equals([NotNull] CodeTemplate other)
        {
            return Equals(ID, other.ID);
        }
    }
}
