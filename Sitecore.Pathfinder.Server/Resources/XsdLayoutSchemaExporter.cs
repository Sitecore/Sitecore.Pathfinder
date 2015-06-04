namespace Sitecore.Pathfinder.Resources
{
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Zip;

  [Export(typeof(IResourceExporter))]
  public class XsdLayoutSchemaExporter : IResourceExporter
  {
    public void Export(ZipWriter zip)
    {
      this.GenerateRenderingXsdSchema(zip, "http://www.sitecore.net/pathfinder/layouts/master", "master");
      this.GenerateRenderingXsdSchema(zip, "http://www.sitecore.net/pathfinder/layouts/core", "core");
    }

    protected void GenerateRenderingXsdSchema([NotNull] ZipWriter zip, [NotNull] string schemaNamespace, [NotNull] string databaseName)
    {
      var generator = new XsdLayoutSchemaGenerator();
      var schema = generator.Generate(schemaNamespace, databaseName);
      zip.AddEntry(".schemas\\" + databaseName + ".layout.xsd", Encoding.UTF8.GetBytes(schema));
    }
  }
}