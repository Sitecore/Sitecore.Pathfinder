// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Helpers;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.IO
{
    [TestClass]
    public class PathHelperTests : Tests
    {
        [TestMethod]
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

        [TestMethod]
        public void GetDirectoryAndFileNameWithoutExtensionsTests()
        {
            Assert.AreEqual("\\sitecore\\client\\test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("\\sitecore\\client\\test.txt"));
            Assert.AreEqual("\\sitecore\\client\\test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("/sitecore/client/test.txt"));
            Assert.AreEqual("test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("test.txt"));
            Assert.AreEqual("test", PathHelper.GetDirectoryAndFileNameWithoutExtensions("test"));
        }

        [TestMethod]
        public void RemapDirectory()
        {
            Assert.AreEqual("c:\\5\\6\\3\\4.txt", PathHelper.RemapDirectory("c:\\1\\2\\3\\4.txt", "c:\\1\\2", "c:\\5\\6"));
        }

        [TestMethod]
        public void GetFileNameWithoutExtensionsTests()
        {
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("\\sitecore\\client\\test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("/sitecore/client/test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("/sitecore/client/test"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("test.txt"));
            Assert.AreEqual("test", PathHelper.GetFileNameWithoutExtensions("test"));
            Assert.AreEqual(string.Empty, PathHelper.GetFileNameWithoutExtensions(string.Empty));
        }

        [TestMethod]
        public void GetItemNameTests()
        {
            Start();

            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, "\\sitecore\\client\\test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, "/sitecore/client/test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, "/sitecore/client/test")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, "test.txt")));
            Assert.AreEqual("test", PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, "test")));
            Assert.ThrowsException<ArgumentException>(() => PathHelper.GetItemName(new SourceFile(Services.Configuration, Services.FileSystem, string.Empty)));
        }

        [TestMethod]
        public void GetItemParentPathTests()
        {
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("\\sitecore\\client\\test"));
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("/sitecore/client/test"));
            Assert.AreEqual("/sitecore/client", PathHelper.GetItemParentPath("/sitecore/client/test/"));
            Assert.AreEqual(string.Empty, PathHelper.GetItemParentPath("/sitecore"));
            Assert.AreEqual(string.Empty, PathHelper.GetItemParentPath(string.Empty));
        }

        /*
        [TestMethod]
        public void GetItemPathTests()
        {
            Start();

            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "\\sitecore\\client\\test.txt", "\\sitecore\\client\\test.txt", "\\sitecore\\client\\test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "/sitecore/client/test.txt", "/sitecore/client/test.txt", "/sitecore/client/test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/client/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "/sitecore/client/test", "/sitecore/client/test", "/sitecore/client/test"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt"), string.Empty, string.Empty));
            Assert.AreEqual("/sitecore/test", PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, "test", "test", "test"), string.Empty, string.Empty));
            Assert.ThrowsException<ArgumentException>(() => PathHelper.GetItemPath(Project.Empty, new SourceFile(Services.FileSystem, string.Empty, string.Empty, string.Empty), string.Empty, string.Empty));
        }
        */

        [TestMethod]
        public void MatchesPattern()
        {
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "*.txt"));
            Assert.IsFalse(PathHelper.MatchesPattern("\\sitecore\\test.txt", "*.cs"));
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "test.txt"));
            Assert.IsTrue(PathHelper.MatchesPattern("\\sitecore\\test.txt", "tes?.txt"));
            Assert.IsFalse(PathHelper.MatchesPattern("\\sitecore\\tett.txt", "tes?.txt"));
        }

        [TestMethod]
        public void NormalizeFilePath()
        {
            Assert.AreEqual("\\sitecore\\test.txt", PathHelper.NormalizeFilePath("\\sitecore\\test.txt"));
            Assert.AreEqual("\\sitecore\\test.txt", PathHelper.NormalizeFilePath("/sitecore/test.txt"));
            Assert.AreEqual("sitecore\\test.txt", PathHelper.NormalizeFilePath("sitecore/test.txt"));
            Assert.AreEqual("test.txt", PathHelper.NormalizeFilePath("test.txt"));
        }

        [TestMethod]
        public void NormalizeItemPath()
        {
            Assert.AreEqual("/sitecore/test.txt", PathHelper.NormalizeItemPath("\\sitecore\\test.txt"));
            Assert.AreEqual("/sitecore/test.txt", PathHelper.NormalizeItemPath("/sitecore/test.txt"));
            Assert.AreEqual("test.txt", PathHelper.NormalizeItemPath("test.txt"));
        }

        [TestMethod]
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
