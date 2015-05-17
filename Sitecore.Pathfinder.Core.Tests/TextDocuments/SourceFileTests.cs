namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.IO;
  using System.Reflection;
  using NUnit.Framework;
  using Sitecore.Pathfinder.IO;

  [TestFixture]
  public class SourceFileTests
  {
    [Test]
    public void ConstructorTest()
    {
      var fileSystem = new FileSystemService();

      var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Website\\content\\Home\\HelloWorld.item.xml");
      var sourceFile = new SourceFile(fileSystem, fileName);

      Assert.AreEqual(fileName, sourceFile.SourceFileName);
      Assert.AreNotEqual(DateTime.MinValue, sourceFile.LastWriteTimeUtc);
    } 
  }
}