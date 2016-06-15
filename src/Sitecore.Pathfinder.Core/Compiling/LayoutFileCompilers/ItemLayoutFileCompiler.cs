// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public class ItemLayoutFileCompiler : CompilerBase
    {
        // must come after RenderingCompiler, or renderings will not be found
        [ImportingConstructor]
        public ItemLayoutFileCompiler([NotNull, ItemNotNull, ImportMany] IEnumerable<ILayoutFileCompiler> layoutFileCompilers) : base(9000)
        {
            LayoutFileCompilers = layoutFileCompilers;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<ILayoutFileCompiler> LayoutFileCompilers { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as ISourcePropertyBag;
            return item != null && item.ContainsSourceProperty(LayoutFileItemParser.LayoutFile);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as ISourcePropertyBag;
            Assert.Cast(item, nameof(item));

            var property = item.GetSourceProperty<string>(LayoutFileItemParser.LayoutFile);
            if (property == null)
            {
                return;
            }

            var compiled = false;
            foreach (var layoutFileCompiler in LayoutFileCompilers)
            {
                if (!layoutFileCompiler.CanCompile(context, projectItem, property))
                {
                    continue;
                }

                layoutFileCompiler.Compile(context, projectItem, property);
                compiled = true;
                break;
            }

            if (!compiled)
            {
                context.Trace.TraceError(Msg.C1063, "Element has a 'Layout.File' attribute, but it could not be compiled", TraceHelper.GetTextNode(property), property.GetValue());
            }
        }
    }
}
