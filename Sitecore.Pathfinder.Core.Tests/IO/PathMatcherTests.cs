namespace Sitecore.Pathfinder.IO
{
  using NUnit.Framework;

  [TestFixture]
  public class PathMatcherTests : Tests
  {
    [Test]
    public void DeepExcludeFolderTests()
    {
      var pathMatcher = new PathMatcher("/folder/**/file.txt", "**/*.cs");

      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.cs"));
    }

    [Test]
    public void DeepFolderTests()
    {
      var pathMatcher = new PathMatcher("/folder/**/file.txt", string.Empty);

      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
    }

    [Test]
    public void FileTests()
    {
      var pathMatcher = new PathMatcher("/folder/file.txt", string.Empty);
      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/file.cs"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.txt"));

      pathMatcher = new PathMatcher("/folder/file.*", string.Empty);
      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.cs"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.txt"));

      pathMatcher = new PathMatcher("/folder/*", string.Empty);
      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.cs"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.txt"));
    }

    [Test]
    public void FolderTests()
    {
      var pathMatcher = new PathMatcher("/folder/*", string.Empty);

      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.txt"));
    }

    [Test]
    public void IncludeTests()
    {
      var pathMatcher = new PathMatcher("**/*", string.Empty);

      Assert.IsTrue(pathMatcher.IsMatch("file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
    }

    [Test]
    public void NestedFolderTests()
    {
      var pathMatcher = new PathMatcher("/folder/*/file.txt", string.Empty);

      Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
      Assert.IsFalse(pathMatcher.IsMatch("/folder/file.txt"));
      Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
    }
  }
}
