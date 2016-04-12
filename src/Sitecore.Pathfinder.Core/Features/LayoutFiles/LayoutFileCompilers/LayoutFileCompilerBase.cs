// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Features.LayoutFiles.LayoutFileCompilers
{
    [InheritedExport]
    public abstract class LayoutFileCompilerBase : ILayoutFileCompiler
    {
        public abstract bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property);

        public abstract void Compile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property);

        protected virtual void CreateLayoutWithRendering([NotNull] ICompileContext context, [NotNull] Item item, Guid layoutId, Guid renderingId, [NotNull] string placeholderKey)
        {
            var writer = new StringWriter();
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartElement("r");

            var deviceItems = item.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase));
            foreach (var deviceItem in deviceItems)
            {
                if (!deviceItem.ItemIdOrPath.StartsWith("/sitecore/layout/Devices/"))
                {
                    continue;
                }

                output.WriteStartElement("d");
                output.WriteAttributeString("id", deviceItem.Uri.Guid.Format());
                output.WriteAttributeString("l", layoutId.Format());

                output.WriteStartElement("r");
                output.WriteAttributeString("id", renderingId.Format());
                output.WriteAttributeString("ph", placeholderKey);
                output.WriteEndElement();

                output.WriteEndElement();
            }

            output.WriteEndElement();

            var field = item.Fields["__Renderings"];
            if (field == null)
            {
                field = context.Factory.Field(item, TextNode.Empty, "__Renderings", writer.ToString());
                item.Fields.Add(field);
            }
            else
            {
                field.Value = writer.ToString();
            }
        }

        protected virtual void RetryCompilation([NotNull] IProjectItem projectItem)
        {
            projectItem.State = ProjectItemState.CompilationPending;
        }
    }
}
