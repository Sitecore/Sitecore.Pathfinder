namespace Sitecore.Pathfinder.Server.Tests.Building.ItemFiles.ItemFileCompilers.Items
{
  using System.ComponentModel.Composition.Hosting;
  using System.IO;
  using System.Linq;
  using System.Xml.Linq;
  using NUnit.Framework;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Building;
  using Sitecore.Pathfinder.Building.Builders.ItemFiles;
  using Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Server.Tests.Data;

  [TestFixture]
  public class ItemParserTests
  {
    [Test]
    public void Bootstrap3AlertTest()
    {
      var traceService = new TraceService();
      var dataService = new TestDataService();
      var compositionService = new CompositionContainer(new AssemblyCatalog(typeof(XmlItemParser).Assembly));
      var buildContext = new BuildContext(compositionService, dataService, traceService, new FileSystemService(traceService), ".");

      var compilerContext = new ItemFileBuildContext(buildContext, null, "Building\\ItemFiles\\ItemFileCompilers\\Items\\Bootstrap3-Alert.item.xml");

      var text = File.ReadAllText(compilerContext.FileName);
      var doc = XDocument.Parse(text, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

      var parser = new XmlItemParser(buildContext.CompositionService);
      var parserContext = new XmlItemParserContext(compilerContext, parser);
      parser.ParseElement(parserContext, doc.Root);
    }
  }
}
