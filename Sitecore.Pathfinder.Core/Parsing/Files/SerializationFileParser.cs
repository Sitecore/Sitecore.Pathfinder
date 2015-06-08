namespace Sitecore.Pathfinder.Parsing.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions;
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

      var tempItem = context.Factory.Item(context.Project, "TempItem", root, string.Empty, string.Empty, string.Empty, string.Empty);
      this.ParseLines(context, tempItem, lines, 0, ref projectUniqueId);

      var item = context.Factory.Item(context.Project, projectUniqueId, root, tempItem.DatabaseName, tempItem.ItemName.Value, tempItem.ItemIdOrPath, tempItem.TemplateIdOrPath.Value);
      item.ItemName.Source = tempItem.ItemName.Source;
      item.TemplateIdOrPath.Source = tempItem.TemplateIdOrPath.Source;
      item.Icon = tempItem.Icon;
      item.IsEmittable = false;

      foreach (var field in tempItem.Fields)
      {
        field.Item = item;
        item.Fields.Add(field);
      }

      context.Project.AddOrMerge(item);

      var serializationFile = context.Factory.SerializationFile(context.Project, context.Snapshot);
      context.Project.AddOrMerge(serializationFile);
    }

    protected virtual int ParseContent([NotNull] string[] lines, int startIndex, int contentLength, out string value, ref int lineLength)
    {
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

    protected virtual int ParseField([NotNull] IParseContext context, [NotNull] Item serializationFile, [NotNull] string[] lines, int lineNumber, [NotNull] string language, int version)
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
        }

        if (name == "content-length")
        {
          var contentLength = int.Parse(value);
          n = this.ParseContent(lines, n + 2, contentLength, out fieldValue, ref lineLength);
          break;
        }
      }

      var field = context.Factory.Field(serializationFile, fieldName, language, version, fieldValue);
      field.Value.Source = context.Factory.TextNode(serializationFile.Snapshot, new TextPosition(lineNumber, 0, lineLength), fieldName, fieldValue, serializationFile.ItemName.Source);

      serializationFile.Fields.Add(field);

      return n;
    }

    protected virtual int ParseLines([NotNull] IParseContext context, [NotNull] Item item, [NotNull] string[] lines, int lineNumber, [NotNull] ref string projectUniqueId)
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
          n = this.ParseField(context, item, lines, n + 1, language, version);
          continue;
        }

        if (line == "----version----")
        {
          n = this.ParseVersion(lines, n + 1, ref language, ref version);
          continue;
        }

        if (line == "----item----")
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
            item.ItemName.SetValue(value);
            item.ItemName.Source = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "name", value, null);
            break;
          case "master":
            break;
          case "template":
            item.TemplateIdOrPath.SetValue(value);
            item.TemplateIdOrPath.Source = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "template", value, null);
            break;
          case "templatekey":
            break;
          case "version":
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