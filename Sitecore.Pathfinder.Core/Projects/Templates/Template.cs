namespace Sitecore.Pathfinder.Projects.Templates
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Items;

  public class Template : ItemBase
  {
    public static readonly Template Empty = new Template(Projects.Project.Empty, "{7A3E077F-D985-453F-8773-348ADFEAF2FD}", Documents.TextNode.Empty);

    public Template([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode document) : base(project, projectUniqueId, document)
    {
    }

    [NotNull]
    public string BaseTemplates { get; set; } = string.Empty;

    [NotNull]
    public string LongHelp { get; set; } = string.Empty;

    [NotNull]
    public IList<TemplateSection> Sections { get; } = new List<TemplateSection>();

    [NotNull]
    public string ShortHelp { get; set; } = string.Empty;

    public void Merge([NotNull] Template newTemplate)
    {
      // todo: throw exception if item and newItem value differ
      if (!string.IsNullOrEmpty(newTemplate.ItemName))
      {
        this.ItemName = newTemplate.ItemName;
      }

      if (!string.IsNullOrEmpty(newTemplate.DatabaseName))
      {
        this.DatabaseName = newTemplate.DatabaseName;
      }

      if (!string.IsNullOrEmpty(newTemplate.BaseTemplates))
      {
        // todo: join base templates
        this.BaseTemplates = newTemplate.BaseTemplates;
      }

      if (!string.IsNullOrEmpty(newTemplate.Icon))
      {
        this.Icon = newTemplate.Icon;
      }

      if (!string.IsNullOrEmpty(newTemplate.ShortHelp))
      {
        this.ShortHelp = newTemplate.ShortHelp;
      }

      if (!string.IsNullOrEmpty(newTemplate.LongHelp))
      {
        this.LongHelp = newTemplate.LongHelp;
      }

      if (!newTemplate.IsEmittable)
      {
        this.IsEmittable = false;
      }

      foreach (var newSection in newTemplate.Sections)
      {
        var section = this.Sections.FirstOrDefault(s => string.Compare(s.Name, newSection.Name, StringComparison.OrdinalIgnoreCase) == 0);
        if (section == null)
        {
          this.Sections.Add(newSection);
          continue;
        }

        section.Merge(newSection);
      }
    }
  }
}