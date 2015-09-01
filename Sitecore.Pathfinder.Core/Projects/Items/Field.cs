// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items
{
    // todo: consider basing this on ProjectElement
    [DebuggerDisplay("{GetType().Name,nq}: {FieldName,nq} = {Value}")]
    public class Field
    {
        private bool _isValid;

        public Field([NotNull] Item item)
        {
            Item = item;
        }

        [NotNull]
        public ICollection<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

        [NotNull]
        public Attribute<string> FieldName { get; } = new Attribute<string>("Name", string.Empty);

        public bool IsResolved { get; set; }

        public bool IsTestable { get; set; } = true;

        public bool IsValid
        {
            get
            {
                if (!IsResolved)
                {
                    Resolve();
                }

                return _isValid;
            }

            protected set { _isValid = value; }
        }

        [NotNull]
        public Item Item { get; set; }

        [NotNull]
        public Attribute<string> Language { get; } = new Attribute<string>("Language", string.Empty);

        [NotNull]
        public string ResolvedValue { get; private set; }

        [NotNull]
        public TemplateField TemplateField => Item.Template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => string.Compare(f.FieldName.Value, FieldName.Value, StringComparison.OrdinalIgnoreCase) == 0) ?? TemplateField.Empty;

        [NotNull]
        public Attribute<string> Value { get; } = new Attribute<string>("Value", string.Empty);

        [NotNull]
        public Attribute<string> ValueHint { get; } = new Attribute<string>("Value.Hint", string.Empty);

        [NotNull]
        public Attribute<int> Version { get; } = new Attribute<int>("Version", 0);

        public void Invalidate()
        {
            IsResolved = false;
            IsValid = false;
        }

        public void Resolve()
        {
            if (IsResolved)
            {
                return;
            }

            IsResolved = true;
            ResolvedValue = Value.Value;

            foreach (var fieldResolver in Item.Project.FieldResolvers.OrderBy(r => r.Priority))
            {
                if (fieldResolver.CanResolve(this))
                {
                    ResolvedValue = fieldResolver.Resolve(this);
                    break;
                }
            }

            IsValid = Diagnostics.All(d => d.Severity != Severity.Error);
        }

        public virtual void WriteDiagnostic(Severity severity, [NotNull] string text, [NotNull] string details = "")
        {
            var source = FieldName.Source ?? TextNode.Empty;
            WriteDiagnostic(severity, text, source, details.Trim());
        }

        public void WriteDiagnostic(Severity severity, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "")
        {
            details = details.Trim();

            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            Diagnostics.Add(new Diagnostic(string.Empty, textNode.Position, severity, text));
        }
    }
}
