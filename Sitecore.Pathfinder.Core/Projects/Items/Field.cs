// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects.Items
{
    // todo: consider basing this on ProjectElement
    [DebuggerDisplay("{GetType().Name,nq}: {FieldName,nq} = {Value}")]
    public class Field
    {
        public Field([NotNull] Item item)
        {
            Item = item;
        }

        [NotNull]
        public Attribute<string> FieldName { get; } = new Attribute<string>("Name", string.Empty);

        public bool IsTestable { get; set; } = true;

        [NotNull]
        public Item Item { get; set; }

        [NotNull]
        public Attribute<string> Language { get; } = new Attribute<string>("Language", string.Empty);

        [NotNull]
        public TemplateField TemplateField => Item.Template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => string.Compare(f.FieldName.Value, FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0) ?? TemplateField.Empty;

        [NotNull]
        public Attribute<string> Value { get; } = new Attribute<string>("Value", string.Empty);

        [NotNull]
        public Attribute<string> ValueHint { get; } = new Attribute<string>("Value.Hint", string.Empty);

        [NotNull]
        public Attribute<int> Version { get; } = new Attribute<int>("Version", 0);

        [NotNull]
        public string GetDatabaseValue(ITraceService trace)
        {
            foreach (var fieldResolver in Item.Project.FieldResolvers.OrderBy(r => r.Priority))
            {
                if (fieldResolver.CanResolve(trace, Item.Project, this))
                {
                    return fieldResolver.Resolve(trace, Item.Project, this);
                }
            }

            return Value;
        }
    }
}
