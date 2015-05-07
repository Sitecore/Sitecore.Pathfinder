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

    public override bool CanEmit(IEmitContext context, ProjectElementBase model)
    {
      // do not apply to inheriting classes
      return model.GetType() == typeof(TemplateModel);
    }

    public override void Emit(IEmitContext context, ProjectElementBase model)
    {
      var itemModel = (TemplateModel)model;

      var templateBuilder = new TemplateBuilder(itemModel);
      templateBuilder.Build(context);
    }
  }
}
