// © 2015 Sitecore Corporation A/S. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Pathfinder.IO
{
    [TestClass]
    public class PathMatcherTests : Tests
    {
        [TestMethod]
        public void DeepExcludeFolderTests()
        {
            var pathMatcher = new PathMatcher("/folder/**/file.txt", "**/*.cs");

            Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
            Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.cs"));
        }

        [TestMethod]
        public void DeepFolderTests()
        {
            var pathMatcher = new PathMatcher("/folder/**/file.txt", string.Empty);

            Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
        }

        [TestMethod]
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

        [TestMethod]
        public void FolderTests()
        {
            var pathMatcher = new PathMatcher("/folder/*", string.Empty);

            Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
            Assert.IsFalse(pathMatcher.IsMatch("/folder/folder/file.txt"));
        }

        [TestMethod]
        public void IncludeTests()
        {
            var pathMatcher = new PathMatcher("**/*", string.Empty);

            Assert.IsTrue(pathMatcher.IsMatch("file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
        }

        [TestMethod]
        public void NestedFolderTests()
        {
            var pathMatcher = new PathMatcher("/folder/*/file.txt", string.Empty);

            Assert.IsFalse(pathMatcher.IsMatch("file.txt"));
            Assert.IsFalse(pathMatcher.IsMatch("/folder/file.txt"));
            Assert.IsTrue(pathMatcher.IsMatch("/folder/folder/file.txt"));
        }
    }
}
