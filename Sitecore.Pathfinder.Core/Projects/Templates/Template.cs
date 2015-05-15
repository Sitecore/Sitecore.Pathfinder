namespace Sitecore.Pathfinder.Projects.Templates
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
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