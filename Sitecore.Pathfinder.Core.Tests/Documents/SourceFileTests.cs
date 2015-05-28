namespace Sitecore.Pathfinder.Documents
{
  using System;
  using System.IO;
  using System.Reflection;
  using NUnit.Framework;
  using Sitecore.Pathfinder.IO;

  [TestFixture]
  public class SourceFileTests : Tests
  {
    [Test]
    public void ConstructorTest()
    {
      this.Start();

      var fileName = Path.Combine(this.ProjectDirectory, "content\\Home\\HelloWorld.item.xml");
      var sourceFile = new SourceFile(this.Services.FileSystem, fileName);

      Assert.AreEqual(fileName, sourceFile.FileName);
      Assert.AreNotEqual(DateTime.MinValue, sourceFile.LastWriteTimeUtc);
    } 
  }
}