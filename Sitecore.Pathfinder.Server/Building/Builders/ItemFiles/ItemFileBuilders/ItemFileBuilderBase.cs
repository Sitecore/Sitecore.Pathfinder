namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ItemFileBuilderBase : IItemFileBuilder
  {
    public const double Items = 9000;

    public const double Layout = 3000;

    public const double Media = 2000;

    public const double Template = 1000;

    protected ItemFileBuilderBase(double priority)
    {
      this.Priority = priority;
    }

    public double Priority { get; }

    public abstract void Build(IItemFileBuildContext context);

    public abstract bool CanBuild(IItemFileBuildContext context);

    [NotNull]
    protected XElement LoadXmlFile([NotNull] IItemFileBuildContext context)
    {
      var text = context.BuildContext.FileSystem.ReadAllText(context.FileName);

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
