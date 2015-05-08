namespace Sitecore.Pathfinder.Projects.Templates
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class Template : Item
  {
    public Template([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.BaseTemplates = string.Empty;
      this.Sections = new List<TemplateSection>();
    }

    [NotNull]
    public string BaseTemplates { get; set; }

    [NotNull]
    public IList<TemplateSection> Sections { get; }
  }
}
