namespace Sitecore.ProjectModel.Data
{
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using NUnit.Framework;
  using Sitecore.Building;
  using Sitecore.Diagnostics;
  using Sitecore.IO;

  [TestFixture]
  public class ProjectItemTests
  {
    [Test]
    public void ReadSerializationFormatTests()
    {
      var configuration = new Configuration();
      var traceService = new TraceService(configuration);
      var context = new BuildContext(configuration, traceService, new CompositionContainer(), new FileSystemService(traceService));

      var text = File.ReadAllText("ProjectModel\\Data\\Test1.item");

      var item = new ProjectItem();
      item.ReadSerialization(context, text);

      Assert.AreEqual("{3660A9CF-559C-4399-8721-055216C66CF7}", item.Id);

      var database = item.Database;
      Assert.IsNotNull(database);
      Assert.AreEqual("core", database.Name);
      Assert.AreEqual("/sitecore/client/Bootstrap3/Content/Bootstrap3/Typography/PageSettings/Sections/Forms", item.Path);
      Assert.AreEqual("{C5D081B4-5362-4A43-A99B-6D206177D0ED}", item.ParentId);
      Assert.AreEqual("Forms", item.Name);
      Assert.AreEqual("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}", item.TemplateId);
      Assert.AreEqual("Folder", item.TemplateName);

      Assert.AreEqual(6, item.Fields.Count);

      Assert.AreEqual("50", item["__Sortorder"]);
      Assert.IsTrue(item["__Renderings"].StartsWith("<r>"));
      Assert.IsTrue(item["__Renderings"].EndsWith("</r>"));
      Assert.AreEqual("20130819T183313", item["__Created"]);

      var field = item.Fields["__Created"];
      Assert.IsNotNull(field);
      Assert.AreEqual("20130819T183313", field.Value);
      Assert.AreEqual("en", field.Language);
      Assert.AreEqual(1, field.Version);
      Assert.AreEqual("{25BED78C-4957-4165-998A-CA1B52F67497}", field.Id);
    }
  }
}