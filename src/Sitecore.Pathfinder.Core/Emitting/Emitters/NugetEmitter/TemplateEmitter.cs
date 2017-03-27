// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitting.Emitters.NugetEmitter
{
    [Export(typeof(IEmitter)), Shared]
    public class TemplateEmitter : EmitterBase
    {
        public TemplateEmitter() : base(Constants.Emitters.Templates)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return context.ProjectEmitter is NugetProjectEmitter && projectItem is Template;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var template = (Template)projectItem;
            var projectEmitter = (NugetProjectEmitter)context.ProjectEmitter;

            if (template.IsEmittable)
            {
                projectEmitter.EmitTemplate(context, template);
            }
        }
    }
}
