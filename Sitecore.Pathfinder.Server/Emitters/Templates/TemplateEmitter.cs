namespace Sitecore.Pathfinder.Emitters.Templates
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IEmitter))]
  public class TemplateEmitter : EmitterBase
  {
    public TemplateEmitter() : base(Templates)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectItem projectItem)
    {
      // do not apply to inheriting classes
      return projectItem is Template;
    }

    public override void Emit(IEmitContext context, ProjectItem projectItem)
    {
      var template = (Template)projectItem;

      var templateBuilder = new TemplateBuilder(template);
      templateBuilder.Build(context);
    }
  }
}
