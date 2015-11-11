// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LinkFieldCompiler : FieldCompilerBase
    {
        public LinkFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "general link", StringComparison.OrdinalIgnoreCase) || string.Equals(field.TemplateField.Type, "link", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var qualifiedName = field.Value.Trim();
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            var item = field.Item.Project.FindQualifiedItem(qualifiedName);
            if (item == null)
            {
                context.Trace.TraceError(Texts.Link_field_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), qualifiedName);
                return string.Empty;
            }

            return $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{item.Uri.Guid.Format()}\" />";
        }
    }
}
