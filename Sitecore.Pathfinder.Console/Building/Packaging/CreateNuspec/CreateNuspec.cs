namespace Sitecore.Pathfinder.Building.Packaging.CreateNuspec
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class CreateNuspec : TaskBase
  {
    public CreateNuspec() : base("create-nuspec")
    {
    }

    public override void Execute(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        return;
      }

      context.Trace.TraceInformation(ConsoleTexts.Text1004);

      var packageFileName = context.Configuration.Get("packaging:nuspec:filename");
      if (string.IsNullOrEmpty(packageFileName))
      {
        throw new BuildException(ConsoleTexts.Text3006);
      }

      var nuspecFileName = PathHelper.Combine(context.OutputDirectory, packageFileName);

      if (context.FileSystem.FileExists(nuspecFileName))
      {
        context.FileSystem.DeleteFile(nuspecFileName);
      }

      if (!context.SourceFiles.Any())
      {
        return;
      }

      this.BuildNuspecFile(context, nuspecFileName);
    }

    protected void BuildNuspecFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName)
    {
      var outputPath = context.OutputDirectory;
      var packageId = context.Configuration.Get("packaging:nuspec:id");
      var packageVersion = context.Configuration.Get("packaging:nuspec:version");
      var title = context.Configuration.Get("packaging:nuspec:title");
      var authors = context.Configuration.Get("packaging:nuspec:authors");
      var owners = context.Configuration.Get("packaging:nuspec:owners");
      var description = context.Configuration.Get("packaging:nuspec:description");
      var summary = context.Configuration.Get("packaging:nuspec:summary");
      var tags = context.Configuration.Get("packaging:nuspec:tags");

      using (var output = new XmlTextWriter(nuspecFileName, Encoding.UTF8))
      {
        output.Formatting = Formatting.Indented;

        output.WriteStartElement("package");

        output.WriteStartElement("metadata");
        output.WriteElementString("id", packageId);
        output.WriteElementString("version", packageVersion);
        output.WriteElementString("title", title);
        output.WriteElementString("authors", authors);
        output.WriteElementString("owners", owners);
        output.WriteElementString("description", description);
        output.WriteElementString("summary", summary);
        output.WriteElementString("tags", tags);

        output.WriteEndElement();

        output.WriteStartElement("files");

        foreach (var sourceFileName in context.SourceFiles)
        {
          var targetFileName = PathHelper.UnmapPath(outputPath, sourceFileName).TrimStart('\\');

          output.WriteStartElement("file");

          output.WriteAttributeString("src", sourceFileName);
          output.WriteAttributeString("target", targetFileName);

          output.WriteEndElement();
        }

        output.WriteEndElement();

        output.WriteEndElement();
      }
    }
  }
}
