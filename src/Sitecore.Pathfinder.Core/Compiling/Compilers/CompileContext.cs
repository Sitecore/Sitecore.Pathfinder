// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    [Export(typeof(ICompileContext))]
    public class CompileContext : ICompileContext
    {
        [ImportingConstructor]
        public CompileContext([NotNull] IFactoryService factory, [ImportMany, NotNull, ItemNotNull] IEnumerable<ICompiler> compilers)
        {
            Factory = factory;
            Compilers = compilers;
        }

        public IEnumerable<ICompiler> Compilers { get; }

        public IFactoryService Factory { get; }

        public IProject Project { get; private set; } 

        public ICompileContext With(IProject project)
        {
            Project = project;
            return this;
        }
    }
}
