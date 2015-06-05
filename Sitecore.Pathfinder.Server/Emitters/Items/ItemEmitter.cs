namespace Sitecore.Pathfinder.Emitters.Items
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Items;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IEmitter))]
  public class ItemEmitter : EmitterBase
  {
    public ItemEmitter() : base(Constants.Emitters.Items)
    {
    }

    public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
    {
      return projectItem is Item;
    }

    public override void Emit(IEmitContext context, IProjectItem projectItem)
    {
      var item = (Item)projectItem;
      if (!item.IsEmittable)
      {
        return;
      }

      var emittableItem = this.GetEmiitableItem(item);

      var itemBuilder = new ItemBuilder();
      itemBuilder.Build(context, emittableItem);
    }

    [NotNull]
    private Item GetEmiitableItem([NotNull] Item sourceItem)
    {
      // todo: transform item here using field resolvers etc...
      return sourceItem;
    }
  }
}
