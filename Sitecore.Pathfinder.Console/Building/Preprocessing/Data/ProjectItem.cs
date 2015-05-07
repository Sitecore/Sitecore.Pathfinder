namespace Sitecore.Pathfinder.Building.Preprocessing.Data
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

  public class ProjectItem
  {
    public ProjectItem()
    {
      this.Name = string.Empty;
      this.DatabaseName = string.Empty;
      this.Id = string.Empty;
      this.ParentId = string.Empty;
      this.Path = string.Empty;
      this.TemplateId = string.Empty;
      this.TemplateName = string.Empty;

      this.Fields = new List<ProjectField>();
    }

    [NotNull]
    public string DatabaseName { get; set; }

    [NotNull]
    public List<ProjectField> Fields { get; }

    [NotNull]
    public string Id { get; set; }

    public string this[string fieldName] => this.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0)?.Value ?? string.Empty;

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public string ParentId { get; set; }

    [NotNull]
    public string Path { get; set; }

    [NotNull]
    public string TemplateId { get; set; }

    [NotNull]
    public string TemplateName { get; set; }

    public void InitializeFromSourceFileName([NotNull] IBuildContext context, [NotNull] string sourceFileName)
    {
      var databaseName = "master";

      var name = System.IO.Path.GetFileNameWithoutExtension(sourceFileName);
      var n = name.IndexOf('.');
      if (n >= 0)
      {
        name = name.Left(n);
      }

      var path = sourceFileName.Left(sourceFileName.Length - 9);
      if (path.StartsWith(context.OutputDirectory, StringComparison.OrdinalIgnoreCase))
      {
        path = path.Mid(context.OutputDirectory.Length + 1);
      }

      if (path.StartsWith("serialization\\", StringComparison.OrdinalIgnoreCase))
      {
        path = path.Mid(13);
      }

      n = path.IndexOf("\\sitecore", StringComparison.OrdinalIgnoreCase);
      if (n > 0)
      {
        databaseName = path.Left(n).TrimStart('\\');
        path = path.Mid(n);
      }

      this.DatabaseName = databaseName;
      this.Name = name;
      this.Path = path.Replace("\\", "/");
      this.TemplateName = name + "Template";
    }

    public void WriteItemXml([NotNull] TextWriter writer)
    {
      using (var output = new XmlTextWriter(writer)
      {
        Formatting = Formatting.Indented
      })
      {
        output.WriteStartElement("Item");
        output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");

        if (!string.IsNullOrEmpty(this.Name))
        {
          output.WriteAttributeString("Name", this.Name);
        }

        /*
        if (this.Database != null)
        {
          output.WriteAttributeString("database", this.Database.Name);
        }
        */

        /*
        if (!string.IsNullOrEmpty(this.Id))
        {
          output.WriteAttributeString("id", this.Id);
        }
        */
        if (!string.IsNullOrEmpty(this.TemplateName))
        {
          output.WriteAttributeString("Template", this.TemplateName);
        }

        /*
        if (!string.IsNullOrEmpty(this.TemplateId))
        {
          output.WriteAttributeString("templateid", this.TemplateId);
        }
        */

        /*
        if (!string.IsNullOrEmpty(this.ParentId))
        {
          output.WriteAttributeString("parentid", this.ParentId);
        }
        */

        /*
        if (!string.IsNullOrEmpty(this.Path))
        {
          output.WriteAttributeString("path", this.Path);
        }
        */
        var icon = this["__Icon"];
        if (!string.IsNullOrEmpty(icon))
        {
          output.WriteAttributeString("Icon", icon);
          this.Fields.Remove(this.Fields.FirstOrDefault(f => string.Compare(f.Name, "__Icon", StringComparison.OrdinalIgnoreCase) == 0));
        }

        /*
        var sortorder = this["__Sortorder"];
        if (!string.IsNullOrEmpty(sortorder))
        {
          output.WriteAttributeString("sortorder", sortorder);
          this.Fields.Remove("__Sortorder");
        }

        var baseTemplates = this["__Base template"];
        if (!string.IsNullOrEmpty(baseTemplates))
        {
          output.WriteAttributeString("BaseTemplates", baseTemplates);
          this.Fields.Remove("__Base template");
        }

        var standardValues = this["__Standard values"];
        if (!string.IsNullOrEmpty(standardValues))
        {
          output.WriteAttributeString("standardvalues", standardValues);
          this.Fields.Remove("__Standard values");
        }

        */
        var sharedfields = this.Fields.Where(f => string.IsNullOrEmpty(f.Language) && f.Version == 0).ToList();
        this.WriteFieldsXml(output, sharedfields);

        var versionedfields = this.Fields.Where(f => !string.IsNullOrEmpty(f.Language) || f.Version != 0).ToList();

        foreach (var language in versionedfields.Select(f => f.Language).Distinct().OrderBy(l => l))
        {
          foreach (var version in versionedfields.Select(f => f.Version).Distinct().OrderBy(v => v))
          {
            output.WriteStartElement("version");
            output.WriteAttributeString("language", language);
            output.WriteAttributeString("version", version.ToString(CultureInfo.InvariantCulture));

            this.WriteFieldsXml(output, versionedfields.Where(f => f.Language == language && f.Version == version));
          }
        }

        output.WriteEndElement();
      }
    }

    protected virtual void WriteFieldXml([NotNull] XmlTextWriter output, [NotNull] ProjectField field)
    {
      output.WriteStartElement("Field");

      output.WriteAttributeString("Name", field.Name);

      /*
      if (!string.IsNullOrEmpty(field.Id))
      {
        output.WriteAttributeString("id", field.Id);
      }
      */
      if (!string.IsNullOrEmpty(field.Type))
      {
        output.WriteAttributeString("Field.Type", field.Type);
      }

      if (field.Value.IndexOf('<') < 0)
      {
        output.WriteValue(field.Value);
      }
      else if (field.Value.IndexOf("]]>", StringComparison.Ordinal) < 0)
      {
        output.WriteRaw(field.Value);
      }
      else
      {
        output.WriteCData(field.Value);
      }

      output.WriteEndElement();
    }

    private void WriteFieldsXml([NotNull] XmlTextWriter output, [NotNull] IEnumerable<ProjectField> fields)
    {
      // write content fields
      foreach (var field in fields.Where(f => !f.Name.StartsWith("__")).OrderBy(f => f.Name))
      {
        this.WriteFieldXml(output, field);
      }

      // write system fields
      foreach (var field in fields.Where(f => f.Name.StartsWith("__")).OrderBy(f => f.Name))
      {
        this.WriteFieldXml(output, field);
      }
    }
  }
}
