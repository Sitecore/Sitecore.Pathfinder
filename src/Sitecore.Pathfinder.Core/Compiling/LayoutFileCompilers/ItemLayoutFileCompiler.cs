// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

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
            var item = projectItem as Item;
            return item != null && item.ContainsProperty(LayoutFileItemParser.LayoutFile);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var property = item.GetProperty<string>(LayoutFileItemParser.LayoutFile);
            if (property == null)
            {
                return;
            }

            foreach (var layoutFileCompiler in LayoutFileCompilers)
            {
                if (layoutFileCompiler.CanCompile(context, projectItem, property))
                {
                    layoutFileCompiler.Compile(context, projectItem, property);
                    break;
                }
            }
        }
    }
}
