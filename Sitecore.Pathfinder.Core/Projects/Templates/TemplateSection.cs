namespace Sitecore.Pathfinder.Projects.Templates
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateSection
  {
    public TemplateSection()
    {
      this.Name = string.Empty;

      this.Fields = new List<TemplateField>();
    }

    [NotNull]
    public IList<TemplateField> Fields { get; }

    [NotNull]
    public string Name { get; set; }
  }
}
