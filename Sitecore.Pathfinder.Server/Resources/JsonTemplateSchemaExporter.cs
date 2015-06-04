namespace Sitecore.Pathfinder.Resources
{
  using System.ComponentModel.Composition;
  using System.Text;
  using Sitecore.Zip;

  [Export(typeof(IResourceExporter))]
  public class JsonTemplateSchemaExporter : IResourceExporter
  {
    public void Export(ZipWriter zip)
    {
      this.GenerateTemplateJsonSchema(zip, "master");
      this.GenerateTemplateJsonSchema(zip, "core");
    }

    protected void GenerateTemplateJsonSchema([NotNull] ZipWriter zip, [NotNull] string databaseName)
    {
      var generator = new JsonTemplateSchemaGenerator();

      var schema = generator.Generate(databaseName);
      zip.AddEntry(".schemas\\" + databaseName + ".content.schema.json", Encoding.UTF8.GetBytes(schema));
    }
  }
}