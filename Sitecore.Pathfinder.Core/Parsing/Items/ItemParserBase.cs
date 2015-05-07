namespace Sitecore.Pathfinder.Parsing.Items
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ItemParserBase : IItemParser
  {
    public const double Items = 9000;

    public const double Layout = 3000;

    public const double Media = 2000;

    public const double Template = 1000;

    protected ItemParserBase(double priority)
    {
      this.Priority = priority;
    }

    public double Priority { get; }

    public abstract bool CanParse(IItemParseContext context);

    public abstract void Parse(IItemParseContext context);

    [NotNull]
    protected XElement LoadXmlFile([NotNull] IItemParseContext context)
    {
      var text = context.ParseContext.FileSystem.ReadAllText(context.FileName);

      XDocument doc;
      try
      {
        doc = XDocument.Parse(text, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
      }
      catch
      {
        throw new BuildException(Texts.Text1000, context.FileName);
      }

      var root = doc.Root;
      if (root == null)
      {
        throw new BuildException(Texts.Text1000, context.FileName);
      }

      return root;
    }
  }
}
