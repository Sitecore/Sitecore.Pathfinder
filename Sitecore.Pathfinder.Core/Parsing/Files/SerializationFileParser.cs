namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IParser))]
  public class SerializationFileParser : ParserBase
  {
    private const string FileExtension = ".item";

    public SerializationFileParser() : base(Constants.Parsers.ContentFiles)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.Snapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = (ITextSnapshot)context.Snapshot;
      var root = textDocument.Root;
      if (root == TextNode.Empty)
      {
        context.Trace.TraceError(Texts.Document_is_not_valid, textDocument.SourceFile.FileName, TextPosition.Empty);
        return;
      }

      var projectUniqueId = context.ItemPath;
      var lines = context.Snapshot.SourceFile.ReadAsLines();

      var tempItem = new Item(context.Project, "TempItem", root);
      this.ParseLines(tempItem, lines, 0, ref projectUniqueId);

      var item = new Item(context.Project, projectUniqueId, root)
      {
        ItemName = tempItem.ItemName,
        ItemIdOrPath = tempItem.ItemIdOrPath,
        DatabaseName = tempItem.DatabaseName,
        TemplateIdOrPath = tempItem.TemplateIdOrPath,
        Icon = tempItem.Icon
      };

      foreach (var field in tempItem.Fields)
      {
        item.Fields.Add(field);
      }

      item = context.Project.AddOrMerge(item);

      var serializationFile = new SerializationFile(context.Project, context.Snapshot, item);
      context.Project.AddOrMerge(serializationFile);
    }

    protected virtual int ParseContent([NotNull] string[] lines, int startIndex, int contentLength, out string value, ref int lineLength)
    {
      value = string.Empty;
      var sb = new StringBuilder();

      for (var n = startIndex; n < lines.Length; n++)
      {
        var line = lines[n];
        lineLength += line.Length;

        if (sb.Length < contentLength)
        {
          sb.Append(line);
          sb.Append("\r\n");
          continue;
        }

        if (!string.IsNullOrEmpty(line))
        {
          value = sb.ToString().Trim().TrimEnd('\n', '\r');
          return n - 1;
        }
      }

      value = sb.ToString().Trim().TrimEnd('\n', '\r');
      return lines.Length;
    }

    protected virtual int ParseField([NotNull] Item serializationFile, [NotNull] string[] lines, int lineNumber, [NotNull] string language, int version)
    {
      var fieldName = string.Empty;
      var fieldValue = string.Empty;
      var lineLength = 0;

      int n;
      for (n = lineNumber; n < lines.Length; n++)
      {
        var line = lines[n];
        lineLength += line.Length;

        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        var i = line.IndexOf(':');
        if (i < 0)
        {
          break;
        }

        var name = line.Left(i).Trim();
        var value = line.Mid(i + 1).Trim();

        switch (name)
        {
          case "field":
            break;
          case "name":
            fieldName = value;
            break;
          case "key":
            break;
          case "content-length":
            var contentLength = int.Parse(value);
            n = this.ParseContent(lines, n + 2, contentLength, out fieldValue, ref lineLength);
            break;
        }
      }

      var field = new Field(fieldName, new Property(new TextNode(serializationFile.Snapshot, new TextPosition(lineNumber, 0, lineLength), string.Empty, fieldValue, serializationFile.ItemTextNode)));
      serializationFile.Fields.Add(field);

      field.Language = language;
      field.Version = version;

      return n;
    }

    protected virtual int ParseLines([NotNull] Item item, [NotNull] string[] lines, int lineNumber, [NotNull] ref string projectUniqueId)
    {
      var language = string.Empty;
      var version = 0;

      for (var n = lineNumber; n < lines.Length; n++)
      {
        var line = lines[n];
        if (string.IsNullOrEmpty(line))
        {
          continue;
        }

        if (line == "----field----")
        {
          n = this.ParseField(item, lines, n + 1, language, version);
          continue;
        }

        if (line == "----version----")
        {
          n = this.ParseVersion(lines, n + 1, ref language, ref version);
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
            projectUniqueId = value;
            break;
          case "database":
            item.DatabaseName = value;
            break;
          case "path":
            item.ItemIdOrPath = value;
            break;
          case "parent":
            break;
          case "name":
            item.ItemName = value;
            break;
          case "master":
            break;
          case "template":
            item.TemplateIdOrPath = value;
            break;
          case "templatekey":
            break;
        }
      }

      return lines.Length;
    }

    protected virtual int ParseVersion([NotNull] string[] lines, int lineNumber, [NotNull] ref string language, ref int version)
    {
      for (var n = lineNumber; n < lines.Length; n++)
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