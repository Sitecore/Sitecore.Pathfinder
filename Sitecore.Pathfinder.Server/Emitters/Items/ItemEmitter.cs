namespace Sitecore.Pathfinder.Emitters.Items
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Items;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IEmitter))]
  public class ItemEmitter : EmitterBase
  {
    public ItemEmitter() : base(Items)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectElementBase model)
    {
      // do not apply to inheriting classes
      return model.GetType() == typeof(ItemModel);
    }

    public override void Emit(IEmitContext context, ProjectElementBase model)
    {
      var itemModel = (ItemModel)model;

      var itemBuilder = new ItemBuilder(itemModel);
      itemBuilder.Build(context);
    }
  }
}
