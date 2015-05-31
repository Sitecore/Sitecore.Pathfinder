namespace Sitecore.Pathfinder.Resources
{
  using System.Text;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Zip;

  public class ResourceManager
  {
    [NotNull]
    public string BuildResourceFile()
    {
      TempFolder.EnsureFolder();

      var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Resources.zip"));
      using (var zip = new ZipWriter(fileName))
      {
        this.GenerateRenderingXsdSchema(zip, "http://www.sitecore.net/pathfinder/layouts/master", "master");
        this.GenerateRenderingXsdSchema(zip, "http://www.sitecore.net/pathfinder/layouts/core", "core");
        this.GenerateRenderingJsonSchema(zip, "master");
        this.GenerateRenderingJsonSchema(zip, "core");
      }

      return fileName;
    }

    protected void GenerateRenderingJsonSchema([NotNull] ZipWriter zip, [NotNull] string databaseName)
    {
      var generator = new JsonSchemaGenerator();
      var schema = generator.Generate(databaseName);
      zip.AddEntry(".schemas\\" + databaseName + ".layout.schema.json", Encoding.UTF8.GetBytes(schema));
    }

    protected void GenerateRenderingXsdSchema([NotNull] ZipWriter zip, [NotNull] string schemaNamespace, [NotNull] string databaseName)
    {
      var generator = new XsdSchemaGenerator();
      var schema = generator.Generate(schemaNamespace, databaseName);
      zip.AddEntry(".schemas\\" + databaseName + "layout.xsd", Encoding.UTF8.GetBytes(schema));
    }
  }
}
