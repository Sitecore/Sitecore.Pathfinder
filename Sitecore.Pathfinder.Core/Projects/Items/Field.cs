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
        public Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] string value, [NotNull] string valueHint = "")
        {
            Item = item;
            FieldName = new Attribute<string>("Name", fieldName);
            Language = new Attribute<string>("Language", language);
            Version = new Attribute<int>("Version", version);
            Value = new Attribute<string>("Value", value);
            ValueHint = valueHint;
        }

        [NotNull]
        public Attribute<string> FieldName { get; }

        [NotNull]
        public Item Item { get; set; }

        [NotNull]
        public Attribute<string> Language { get; }

        public TemplateField TemplateField => Item.Template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => string.Compare(f.Name, FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0) ?? TemplateField.Empty;

        [NotNull]
        public Attribute<string> Value { get; }

        [NotNull]
        public string ValueHint { get; }

        [NotNull]
        public Attribute<int> Version { get; }
    }
}
