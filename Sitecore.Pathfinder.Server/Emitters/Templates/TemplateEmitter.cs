namespace Sitecore.Pathfinder.Emitters.Templates
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Models;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IEmitter))]
  public class TemplateEmitter : EmitterBase
  {
    public TemplateEmitter() : base(Templates)
    {
    }

    public override bool CanEmit(IEmitContext context, ModelBase model)
    {
      // do not apply to inheriting classes
      return model.GetType() == typeof(TemplateModel);
    }

    public override void Emit(IEmitContext context, ModelBase model)
    {
      var itemModel = (TemplateModel)model;

      var templateBuilder = new TemplateBuilder(itemModel);
      templateBuilder.Build(context);
    }
  }
}
