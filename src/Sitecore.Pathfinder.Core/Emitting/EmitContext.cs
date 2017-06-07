// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Emitting
{
    [Export(typeof(IEmitContext))]
    public class EmitContext : IEmitContext
    {
        public ICollection<OutputFile> OutputFiles { get; } = new List<OutputFile>();

        public IProjectBase Project { get; private set; } = Projects.Project.Empty;

        public IProjectEmitter ProjectEmitter { get; protected set; }

        public virtual IEmitContext With(IProjectEmitter projectEmitter, IProjectBase project)
        {
            ProjectEmitter = projectEmitter;
            Project = project;

            return this;
        }
    }
}
