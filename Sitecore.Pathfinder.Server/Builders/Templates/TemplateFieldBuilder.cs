namespace Sitecore.Pathfinder.Builders.Templates
{
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Templates;

  public class TemplateFieldBuilder
  {
    public TemplateFieldBuilder([NotNull] TemplateFieldModel templaterFieldModel)
    {
      this.TemplaterFieldModel = templaterFieldModel;
    }

    [CanBeNull]
    public Item Item { get; set; }

    [NotNull]
    public TemplateFieldModel TemplaterFieldModel { get; }

    public void ResolveItem([NotNull] IEmitContext context, [CanBeNull] Item sectionItem)
    {
      if (this.Item == null && sectionItem != null)
      {
        this.Item = sectionItem.Children[this.TemplaterFieldModel.Name];
      }
    }
  }
}
