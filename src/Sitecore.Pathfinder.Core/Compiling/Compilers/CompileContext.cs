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
        [FactoryConstructor]
        [ImportingConstructor]
        public CompileContext([NotNull] IFactory factory, [ImportMany, NotNull, ItemNotNull] IEnumerable<ICompiler> compilers)
        {
            Factory = factory;
            Compilers = compilers;
        }

        public IEnumerable<ICompiler> Compilers { get; }

        public IFactory Factory { get; }

        public IProject Project { get; private set; } = (IProject) Projects.Project.Empty;

        public ICompileContext With(IProject project)
        {
            Project = project;
            return this;
        }
    }
}
