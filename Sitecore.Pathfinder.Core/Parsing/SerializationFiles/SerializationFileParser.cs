namespace Sitecore.Pathfinder.Parsing.SerializationFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.SerializationFiles;

  [Export(typeof(IParser))]
  public class SerializationFileParser : ParserBase
  {
    private const string FileExtension = ".item";

    public SerializationFileParser() : base(ContentFiles)
    {
    }

    public override bool CanParse(IParseContext context, ISourceFile sourceFile)
    {
      return sourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context, ISourceFile sourceFile)
    {
      var serializationFile = new SerializationFile(sourceFile);
      context.Project.Elements.Add(serializationFile);

      var lines = sourceFile.ReadAllLines();

      this.ReadSerializationItem(serializationFile, lines, 0);
    }

    protected virtual int ReadSerializationField([NotNull] SerializationFile serializationFile, [NotNull] string[] lines, int startIndex, [NotNull] string language, int version)
    {
      var field = new FieldModel(serializationFile.SourceFile);
      serializationFile.Fields.Add(field);

      field.Language = language;
      field.Version = version;

      for (var n = startIndex; n < lines.Length; n++)
      {
        var line = lines[n];
        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        var i = line.IndexOf(':');
        if (i < 0)
        {
          return n;
        }

        var name = line.Left(i).Trim();
        var value = line.Mid(i + 1).Trim();

        switch (name)
        {
          case "field":
            break;
          case "name":
            field.Name = value;
            break;
          case "key":
            break;
          case "content-length":
            var contentLength = int.Parse(value);
            n = this.ReadSerializationFieldContent(field, lines, n + 2, contentLength);
            return n;
        }
      }

      return lines.Length;
    }

    protected virtual int ReadSerializationFieldContent([NotNull] FieldModel field, [NotNull] string[] lines, int startIndex, int contentLength)
    {
      var sb = new StringBuilder();

      for (var n = startIndex; n < lines.Length; n++)
      {
        var line = lines[n];

        if (sb.Length < contentLength)
        {
          sb.Append(line);
          sb.Append("\r\n");
          continue;
        }

        if (!string.IsNullOrEmpty(line))
        {
          field.Value = sb.ToString().Trim().TrimEnd('\n', '\r');
          return n - 1;
        }
      }

      field.Value = sb.ToString().Trim().TrimEnd('\n', '\r');
      return lines.Length;
    }

    protected virtual int ReadSerializationItem([NotNull] SerializationFile serializationFile, [NotNull] string[] lines, int startIndex)
    {
      var language = string.Empty;
      var version = 0;

      for (var n = startIndex; n < lines.Length; n++)
      {
        var line = lines[n];
        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        if (line == "----field----")
        {
          n = this.ReadSerializationField(serializationFile, lines, n + 1, language, version);
          continue;
        }

        if (line == "----version----")
        {
          n = this.ReadSerializationVersion(lines, n + 1, ref language, ref version);
          continue;
        }

        var i = line.IndexOf(':');
        if (i < 0)
        {
          return n;
        }

        var name = line.Left(i).Trim();
        var value = line.Mid(i + 1).Trim();

        switch (name)
        {
          case "id":
            break;
          case "database":
            serializationFile.DatabaseName = value;
            break;
          case "path":
            serializationFile.ItemIdOrPath = value;
            break;
          case "parent":
            break;
          case "name":
            serializationFile.Name = value;
            break;
          case "master":
            break;
          case "template":
            serializationFile.TemplateIdOrPath = value;
            break;
          case "templatekey":
            break;
        }
      }

      return lines.Length;
    }

    protected virtual int ReadSerializationVersion([NotNull] string[] lines, int startIndex, [NotNull] ref string language, ref int version)
    {
      for (var n = startIndex; n < lines.Length; n++)
      {
        var line = lines[n];
        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        var i = line.IndexOf(':');
        if (i < 0)
        {
          return n - 1;
        }

        var name = line.Left(i).Trim();
        var value = line.Mid(i + 1).Trim();

        switch (name)
        {
          case "language":
            language = value;
            break;
          case "version":
            version = int.Parse(value);
            break;
          case "revision":
            break;
        }
      }

      return lines.Length;
    }
  }
}
