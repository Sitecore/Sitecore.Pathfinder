namespace Sitecore.Pathfinder.Emitters.Items
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IEmitter))]
  public class TemplateEmitter : EmitterBase
  {
    public TemplateEmitter() : base(Constants.Emitters.Templates)
    {
    }

    public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
    {
      // do not apply to inheriting classes
      return projectItem is Template;
    }

    public override void Emit(IEmitContext context, IProjectItem projectItem)
    {
      var template = (Template)projectItem;

      var templateBuilder = new TemplateBuilder(template);
      templateBuilder.Build(context);
    }
  }
}
