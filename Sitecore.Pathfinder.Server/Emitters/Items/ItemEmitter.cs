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

      var itemBuilder = new ItemBuilder(item);
      itemBuilder.Build(context);
    }
  }
}
