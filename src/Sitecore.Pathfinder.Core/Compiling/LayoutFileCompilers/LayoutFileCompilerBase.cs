// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public abstract class LayoutFileCompilerBase : ILayoutFileCompiler
    {
        [NotNull]
        protected IFactory Factory { get; }

        protected LayoutFileCompilerBase([NotNull] IFactory factory)
        {
            Factory = factory;
        }

        public abstract bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property);

        public abstract void Compile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property);

        protected virtual void CreateLayout([NotNull] ICompileContext context, [NotNull] Item item, Guid layoutId)
        {
            var writer = new StringBuilder();
            using (var output = Factory.XmlWriter(writer))
            {
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

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }

            SetRenderingsField(context, item, writer.ToString());
        }

        protected virtual void CreateLayoutWithRendering([NotNull] ICompileContext context, [NotNull] Item item, Guid layoutId, Guid renderingId, [NotNull] string placeholderKey)
        {
            var writer = new StringBuilder();
            using (var output = Factory.XmlWriter(writer))
            {
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
            }

            SetRenderingsField(context, item, writer.ToString());
        }

        protected virtual void RetryCompilation([NotNull] IProjectItem projectItem)
        {
            projectItem.CompilationState = CompilationState.Pending;
        }

        protected void SetRenderingsField([NotNull] ICompileContext context, [NotNull] Item item, [NotNull] string value)
        {
            var field = item.Fields["__Renderings"];
            if (field == null)
            {
                field = Factory.Field(item, "__Renderings", value);
                item.Fields.Add(field);
            }
            else
            {
                field.Value = value;
            }
        }
    }
}
