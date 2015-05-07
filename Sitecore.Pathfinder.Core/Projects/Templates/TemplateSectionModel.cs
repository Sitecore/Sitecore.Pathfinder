namespace Sitecore.Pathfinder.Projects.Templates
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateSectionModel
  {
    public TemplateSectionModel()
    {
      this.Name = string.Empty;

      this.Fields = new List<TemplateFieldModel>();
    }

    [NotNull]
    public IList<TemplateFieldModel> Fields { get; }

    [NotNull]
    public string Name { get; set; }
  }
}
