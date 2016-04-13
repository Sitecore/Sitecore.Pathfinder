// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Compiling.LayoutFileCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Html.Html
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

            var fileName = item.GetValue<string>(LayoutFileItemParser.LayoutFile);
            return !string.IsNullOrEmpty(fileName) && string.Equals(PathHelper.GetExtension(fileName), ".html", StringComparison.OrdinalIgnoreCase);
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
                context.Trace.TraceError(Msg.C1060, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), value);
                return;
            }

            var renderingItemUri = rendering.RenderingItemUri;
            if (renderingItemUri == ProjectItemUri.Empty)
            {
                RetryCompilation(projectItem);
                return;
            }

            CreateLayout(context, item, renderingItemUri.Guid);
        }
    }
}
