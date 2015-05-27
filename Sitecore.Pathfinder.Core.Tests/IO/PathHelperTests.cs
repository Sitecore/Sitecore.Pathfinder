namespace Sitecore.Pathfinder.IO
{
  using NUnit.Framework;

  [TestFixture]
  public class PathHelperTests
  {
    [Test]
    public void CombineTests()
    {
      Assert.AreEqual("\\sitecore\\content", PathHelper.Combine("\\sitecore", "content"));
      Assert.AreEqual("\\sitecore\\content", PathHelper.Combine("/sitecore", "content"));
      Assert.AreEqual("\\content", PathHelper.Combine("/sitecore", "/content"));
      Assert.AreEqual("c:\\sitecore\\content", PathHelper.Combine("c:\\sitecore", "content"));
      Assert.AreEqual("content", PathHelper.Combine(string.Empty, "content"));
      Assert.AreEqual("sitecore", PathHelper.Combine("sitecore", string.Empty));
      Assert.AreEqual("\\sitecore", PathHelper.Combine("\\sitecore\\content", ".."));
      Assert.AreEqual("\\sitecore\\Home", PathHelper.Combine("\\sitecore\\content", "..\\Home"));
      Assert.AreEqual("Home", PathHelper.Combine(".", "Home"));
      Assert.AreEqual(string.Empty, PathHelper.Combine(string.Empty, string.Empty));
    }
  }
}