namespace Sitecore.Pathfinder.Parsing.Items.SerializationFileParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Models.Items;
  using Sitecore.Pathfinder.Models.SerializationFiles;

  [Export(typeof(IItemParser))]
  public class SerializationFileParser : ItemParserBase
  {
    private const string FileExtension = ".item";

    public SerializationFileParser() : base(Items)
    {
    }

    public override bool CanParse(IItemParseContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var itemModel = new SerializationFileModel(context.FileName);
      context.ParseContext.Project.Models.Add(itemModel);

      itemModel.SerializationFile = context.FileName;

      // todo: convert to lazy read
      var lines = context.ParseContext.FileSystem.ReadAllLines(context.FileName);
      this.ReadSerializationItem(itemModel, lines, 0);
    }

    protected virtual int ReadSerializationField([NotNull] ItemModel itemModel, [NotNull] string[] lines, int startIndex, [NotNull] string language, int version)
    {
      var field = new FieldModel(itemModel.SourceFileName);
      itemModel.Fields.Add(field);

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

    protected virtual int ReadSerializationItem([NotNull] ItemModel itemModel, [NotNull] string[] lines, int startIndex)
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
          n = this.ReadSerializationField(itemModel, lines, n + 1, language, version);
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
            itemModel.DatabaseName = value;
            break;
          case "path":
            itemModel.ItemIdOrPath = value;
            break;
          case "parent":
            break;
          case "name":
            itemModel.Name = value;
            break;
          case "master":
            break;
          case "template":
            itemModel.TemplateIdOrPath = value;
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
