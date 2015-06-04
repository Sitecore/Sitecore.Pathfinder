namespace Sitecore.Pathfinder.Resources
{
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Zip;

  [Export(typeof(IResourceExporter))]
  public class JsonLayoutSchemaExporter : IResourceExporter
  {
    public void Export(ZipWriter zip)
    {
      this.GenerateRenderingJsonSchema(zip, "master");
      this.GenerateRenderingJsonSchema(zip, "core");
    }

    protected void GenerateRenderingJsonSchema([NotNull] ZipWriter zip, [NotNull] string databaseName)
    {
      var generator = new JsonLayoutSchemaGenerator();
      var schema = generator.Generate(databaseName);
      zip.AddEntry(".schemas\\" + databaseName + ".layout.schema.json", Encoding.UTF8.GetBytes(schema));
    }
  }
}