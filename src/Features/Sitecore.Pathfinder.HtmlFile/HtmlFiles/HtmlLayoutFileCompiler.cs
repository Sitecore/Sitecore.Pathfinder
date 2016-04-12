// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Features.LayoutFiles;
using Sitecore.Pathfinder.Features.LayoutFiles.LayoutFileCompilers;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.HtmlFile.HtmlFiles
{
    public class HtmlLayoutFileCompiler : LayoutFileCompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            if (!item.ContainsProperty(LayoutFileItemParser.LayoutFile))
            {
                return false;
            }

            var extension = item.GetValue<string>(LayoutFileItemParser.LayoutFile);
            return !string.IsNullOrEmpty(extension) && string.Equals(PathHelper.GetExtension(extension), ".html", StringComparison.OrdinalIgnoreCase);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var value = property.GetValue().Trim();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var rendering = item.Project.ProjectItems.OfType<Rendering>().FirstOrDefault(i => string.Equals(i.FilePath, value, StringComparison.OrdinalIgnoreCase));
            if (rendering == null)
            {
                context.Trace.TraceError(Msg.C1047, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), value);
            }

            var layoutItem = item.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => string.Equals(i.ItemIdOrPath, Constants.Layouts.MvcLayout, StringComparison.OrdinalIgnoreCase));
            if (layoutItem == null)
            {
                context.Trace.TraceError(Msg.C1048, Texts.Layout_reference_not_found, TraceHelper.GetTextNode(property), Constants.Layouts.MvcLayout);
            }

            if (rendering == null || layoutItem == null)
            {
                return;
            }

            var renderingItemUri = rendering.RenderingItemUri;
            if (renderingItemUri == ProjectItemUri.Empty)
            {
                RetryCompilation(projectItem);
                return;
            }

            CreateLayoutWithRendering(context, item, layoutItem.Uri.Guid, renderingItemUri.Guid, "Page.Body");
        }
    }
}
