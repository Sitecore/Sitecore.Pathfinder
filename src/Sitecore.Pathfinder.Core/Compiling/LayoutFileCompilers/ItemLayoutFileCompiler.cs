// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    [Export(typeof(ICompiler)), Shared]
    public class ItemLayoutFileCompiler : CompilerBase
    {
        // must come after RenderingCompiler, or renderings will not be found
        [ImportingConstructor]
        public ItemLayoutFileCompiler([NotNull] ITraceService trace, [NotNull, ItemNotNull, ImportMany] IEnumerable<ILayoutFileCompiler> layoutFileCompilers) : base(9000)
        {
            Trace = trace;
            LayoutFileCompilers = layoutFileCompilers;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<ILayoutFileCompiler> LayoutFileCompilers { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            return ((ISourcePropertyBag)item).ContainsSourceProperty(LayoutFileItemParser.LayoutFile);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var sourcePropertyBag = projectItem as ISourcePropertyBag;
            Assert.Cast(sourcePropertyBag, nameof(sourcePropertyBag));

            var property = sourcePropertyBag.GetSourceProperty<string>(LayoutFileItemParser.LayoutFile);
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
                Trace.TraceError(Msg.C1063, Texts.Element_has_a__Layout_File__attribute__but_it_could_not_be_compiled, TraceHelper.GetTextNode(property), property.GetValue());
            }
        }
    }
}
