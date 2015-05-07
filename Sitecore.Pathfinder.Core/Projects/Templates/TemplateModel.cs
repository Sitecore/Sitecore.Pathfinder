namespace Sitecore.Pathfinder.Projects.Templates
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateModel : ItemBase
  {
    public TemplateModel([NotNull] string sourceFileName) : base(sourceFileName)
    {
      this.BaseTemplates = string.Empty;
      this.Sections = new List<TemplateSectionModel>();
    }

    [NotNull]
    public string BaseTemplates { get; set; }

    [NotNull]
    public IList<TemplateSectionModel> Sections { get; }
  }
}
