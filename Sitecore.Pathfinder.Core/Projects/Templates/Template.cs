namespace Sitecore.Pathfinder.Projects.Templates
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class Template : ItemBase
  {
    public Template([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode) : base(project, projectUniqueId, textNode)
    {
      this.Sections = new List<TemplateSection>();
    }

    [NotNull]
    public string BaseTemplates { get; set; } = string.Empty;

    [NotNull]
    public string LongHelp { get; set; } = string.Empty;

    [NotNull]
    public IList<TemplateSection> Sections { get; }

    [NotNull]
    public string ShortHelp { get; set; } = string.Empty;

    public override void Bind()
    {
      base.Bind();

      if (string.IsNullOrEmpty(this.BaseTemplates))
      {
        return;
      }

      var templates = this.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
      foreach (var template in templates)
      {
        this.References.AddTemplateReference(template);
      }
    }
  }
}