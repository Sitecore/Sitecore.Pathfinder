// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler))]
    public class LinkFieldCompiler : FieldCompilerBase
    {
        public LinkFieldCompiler() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Compare(field.TemplateField.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(field.TemplateField.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
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
                context.Trace.TraceError("Link field reference not found", qualifiedName);
                return string.Empty;
            }

            return $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{item.Uri.Guid.Format()}\" />";
        }
    }
}
