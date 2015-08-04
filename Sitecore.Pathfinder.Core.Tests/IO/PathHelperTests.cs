// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.IO
{
    [TestFixture]
    public class PathHelperTests : Tests
    {
        [Test]
        public void CombineTests()
        {
            Assert.AreEqual("\\sitecore\\content", PathHelper.Combine("\\sitecore", "content"));
            Assert.AreEqual("\\sitecore\\content", PathHelper.Combine("/sitecore", "content"));
            Assert.AreEqual("sitecore\\content", PathHelper.Combine("sitecore", "content"));
            Assert.AreEqual("\\content", PathHelper.Combine("/sitecore", "/content"));
            Assert.AreEqual("c:\\sitecore\\content", PathHelper.Combine("c:\\sitecore", "content"));
            Assert.AreEqual("content", PathHelper.Combine(string.Empty, "content"));
            Assert.AreEqual("sitecore", PathHelper.Combine("sitecore", string.Empty));
            Assert.AreEqual("\\sitecore", PathHelper.Combine("\\sitecore\\content", ".."));
            Assert.AreEqual("\\sitecore\\Home", PathHelper.Combine("\\sitecore\\content", "..\\Home"));
            Assert.AreEqual("Home", PathHelper.Combine(".", "Home"));
            Assert.AreEqual(string.Empty, PathHelper.Combine(string.Empty, string.Empty));
        }

        [Test]
        public void GetDirectoryAndFileNameWithoutExtensionsTests()
        {
            Assert.AreEqual("\\sitecore\\client\\test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("\\sitecore\\client\\test.txt"));
            Assert.AreEqual("\\sitecore\\client\\test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("/sitecore/client/test.txt"));
            Assert.AreEqual("test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("test.txt"));
            Assert.AreEqual("test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("test"));
        }

        [Test]
        public void GetFileNameWithoutExtensionsTests()
        {
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("\\sitecore\\client\\test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("/sitecore/client/test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("/sitecore/client/test"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("test"));
            Assert.AreEqual(string.Empty, PathHelper.GetFileNameWithoutExtensions(string.Empty));
        }

        [Test]
        public void GetItemNameTests()
        {
            Start();

            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.FileSystem, "\\sitecore\\client\\test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.FileSystem, "/sitecore/client/test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.FileSystem, "/sitecore/client/test")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.FileSystem, "test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.FileSystem, "test")));
            Assert.Throws<ArgumentException>(() => PathHelper.GetItemName(new SourceFile(Services.FileSystem, string.Empty)));
        }

        [Test]
        public void GetItemParentPathTests()
        {
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("\\sitecore\\client\\test"));
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("/sitecore/client/test"));
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("/sitecore/client/test/"));
            Assert.AreEqual(string.Empty, PathHelper.GetItemParentPath("/sitecore"));
            Assert.AreEqual(string.Empty, PathHelper.GetItemParentPath(string.Empty));
        }

        [Test]
        public void GetItemPathTests()
        {
            Start();

            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "\\sitecore\\client\\test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "/sitecore/client/test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "/sitecore/client/test"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "test"), string.Empty, string.Empty));
            Assert.Throws<ArgumentException>(() => PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, string.Empty), string.Empty, string.Empty));
        }

        [Test]
        public void MatchesPattern()
        {
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "*.txt"));
            Assert.IsFalse(PathHelper.MatchesPattern("\\sitecore\\test.txt", "*.cs"));
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "test.txt"));
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "tes?.txt"));
            Assert.IsFalse(PathHelper.MatchesPattern("\\sitecore\\tett.txt", "tes?.txt"));
        }

        [Test]
        public void NormalizeFilePath()
        {
            Assert.AreEqual("\\sitecore\\test.txt", PathHelper.NormalizeFilePath("\\sitecore\\test.txt"));
            Assert.AreEqual("\\sitecore\\test.txt", PathHelper.NormalizeFilePath("/sitecore/test.txt"));
            Assert.AreEqual("sitecore\\test.txt", PathHelper.NormalizeFilePath("sitecore/test.txt"));
            Assert.AreEqual("test.txt", PathHelper.NormalizeFilePath("test.txt"));
        }

        [Test]
        public void NormalizeItemPath()
        {
            Assert.AreEqual("/sitecore/test.txt", PathHelper.NormalizeItemPath("\\sitecore\\test.txt"));
            Assert.AreEqual("/sitecore/test.txt", PathHelper.NormalizeItemPath("/sitecore/test.txt"));
            Assert.AreEqual("test.txt", PathHelper.NormalizeItemPath("test.txt"));
        }

        [Test]
        public void UnmapPath()
        {
            Start();

            Assert.AreEqual("client\\Home", PathHelper.UnmapPath("c:\\sitecore\\project", "c:\\sitecore\\project\\client\\Home"));
            Assert.AreEqual("client\\Home", PathHelper.UnmapPath("c:\\sitecore\\project", "c:\\sitecore\\project/client/Home"));
            Assert.AreEqual("client\\Home", PathHelper.UnmapPath("c:\\sitecore\\project", "client\\Home"));
            Assert.AreEqual("c:\\solution\\project\\client\\Home", PathHelper.UnmapPath("c:\\sitecore\\project", "c:\\solution\\project\\client\\Home"));
            Assert.AreEqual(string.Empty, PathHelper.UnmapPath("c:\\sitecore\\project", "c:\\sitecore\\project"));
        }
    }
}
