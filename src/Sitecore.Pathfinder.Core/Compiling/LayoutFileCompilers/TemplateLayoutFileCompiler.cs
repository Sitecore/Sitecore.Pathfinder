// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    public class TemplateLayoutFileCompiler : CompilerBase
    {
        // must come after RenderingCompiler, or renderings will not be found
        [ImportingConstructor]
        public TemplateLayoutFileCompiler([NotNull, ItemNotNull, ImportMany] IEnumerable<ILayoutFileCompiler> layoutFileCompilers) : base(9000)
        {
            LayoutFileCompilers = layoutFileCompilers;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<ILayoutFileCompiler> LayoutFileCompilers { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var template = projectItem as Template;
            if (template == null)
            {
                return false;
            }

            return ((ISourcePropertyBag)projectItem).ContainsSourceProperty(LayoutFileItemParser.LayoutFile);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var template = projectItem as Template;
            Assert.Cast(template, nameof(template));

            var property = ((ISourcePropertyBag)template).GetSourceProperty<string>(LayoutFileItemParser.LayoutFile);
            if (property == null)
            {
                return;
            }

            var standardValuesItem = template.StandardValuesItem;
            if (standardValuesItem == null)
            {
                return;
            }

            foreach (var layoutFileCompiler in LayoutFileCompilers)
            {
                if (layoutFileCompiler.CanCompile(context, standardValuesItem, property))
                {
                    layoutFileCompiler.Compile(context, standardValuesItem, property);
                    break;
                }
            }
        }
    }
}
