using System;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public class AspxLayoutFileCompiler : LayoutFileCompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as ISourcePropertyBag;
            if (item == null)
            {
                return false;
            }

            if (!item.ContainsSourceProperty(LayoutFileItemParser.LayoutFile))
            {
                return false;
            }

            var extension = item.GetValue<string>(LayoutFileItemParser.LayoutFile);
            return !string.IsNullOrEmpty(extension) && string.Equals(PathHelper.GetExtension(extension), ".aspx", StringComparison.OrdinalIgnoreCase);
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

            var renderings = item.Project.FindFiles<Rendering>(value).ToList();
            if (!renderings.Any())
            {
                context.Trace.TraceError(Msg.C1060, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), value);
                return;
            }
            if (renderings.Count > 1)
            {
                context.Trace.TraceError(Msg.C1062, "Ambiguous file name", TraceHelper.GetTextNode(property), value);
                return;
            }

            var rendering = renderings.First();

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