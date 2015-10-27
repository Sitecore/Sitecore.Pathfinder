// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitters.Items
{
    public class TemplateEmitter : EmitterBase
    {
        public TemplateEmitter() : base(Constants.Emitters.Templates)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is Template;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var template = (Template)projectItem;
            if (!template.IsEmittable || template.IsExtern)
            {
                return;
            }

            var templateWriter = new TemplateWriter(template);
            templateWriter.Write(context);
        }
    }
}
