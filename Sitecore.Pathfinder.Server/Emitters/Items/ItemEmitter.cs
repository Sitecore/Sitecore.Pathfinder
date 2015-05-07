namespace Sitecore.Pathfinder.Emitters.Items
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Items;
  using Sitecore.Pathfinder.Models;
  using Sitecore.Pathfinder.Models.Items;

  [Export(typeof(IEmitter))]
  public class ItemEmitter : EmitterBase
  {
    public ItemEmitter() : base(Items)
    {
    }

    public override bool CanEmit(IEmitContext context, ModelBase model)
    {
      // do not apply to inheriting classes
      return model.GetType() == typeof(ItemModel);
    }

    public override void Emit(IEmitContext context, ModelBase model)
    {
      var itemModel = (ItemModel)model;

      var itemBuilder = new ItemBuilder(itemModel);
      itemBuilder.Build(context);
    }
  }
}
