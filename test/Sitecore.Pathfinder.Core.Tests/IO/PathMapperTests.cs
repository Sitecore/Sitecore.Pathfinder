// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;
using Sitecore.Pathfinder.IO.PathMappers;

namespace Sitecore.Pathfinder.IO
{
    [TestFixture]
    public class PathMapperTests : Tests
    {
        [Test]
        public void ProjectFileNameToWebsiteFileNameTests()
        {
            var pathMapperService = new PathMapperService();

            pathMapperService.ProjectDirectoryToWebsiteDirectories.Add(new ProjectDirectoryToWebsiteDirectoryMapper("/wwwroot", "/", "**/*.html", "**/*.aspx"));

            string websiteFileName;
            Assert.IsTrue(pathMapperService.TryGetWebsiteFileName("/wwwroot/about.html", out websiteFileName));
            Assert.AreEqual("~/about.html", websiteFileName);

            Assert.IsTrue(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.html", out websiteFileName));
            Assert.AreEqual("~/files/about.html", websiteFileName);

            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.aspx", out websiteFileName));
            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.ashx", out websiteFileName));
            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/files/about.html", out websiteFileName));
        }

        [Test]
        public void ProjectFileNameToWebsiteItemPathTests()
        {
            var pathMapperService = new PathMapperService();

            pathMapperService.ProjectDirectoryToWebsiteItemPaths.Add(new ProjectDirectoryToWebsiteItemPathMapper("/content/master/sitecore", "master", "/sitecore", "**/*.item.json", "**/*.aspx", false, true));

            string itemPath;
            string databaseName;
            bool isImport;
            bool uploadMedia;
            Assert.IsTrue(pathMapperService.TryGetWebsiteItemPath("/content/master/sitecore/content/Home/about.item.json", out databaseName, out itemPath, out isImport, out uploadMedia));
            Assert.AreEqual("/sitecore/content/Home/about", itemPath);

            Assert.IsFalse(pathMapperService.TryGetWebsiteItemPath("/content/master/sitecore/content/Home/about.item.xml", out databaseName, out itemPath, out isImport, out uploadMedia));
            Assert.IsFalse(pathMapperService.TryGetWebsiteItemPath("/content/master/sitecore/content/Home/about.aspx", out databaseName, out itemPath, out isImport, out uploadMedia));
            Assert.IsFalse(pathMapperService.TryGetWebsiteItemPath("/content/core/sitecore/content/Home/about.aspx", out databaseName, out itemPath, out isImport, out uploadMedia));
        }

        [Test]
        public void WebsiteFileNameToProjectFileNameTests()
        {
            var pathMapperService = new PathMapperService();

            pathMapperService.WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper("/sitecore modules/shell/module", "/module", "**/*.cshtml", "**/*.png"));
            pathMapperService.WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper("/", "/wwwroot", "**/*.cshtml", "**/*.png"));

            string projectFileName;
            Assert.IsTrue(pathMapperService.TryGetProjectFileName("/sitecore modules/shell/module/overview.cshtml", out projectFileName));
            Assert.AreEqual("module\\overview.cshtml", projectFileName);

            Assert.IsTrue(pathMapperService.TryGetProjectFileName("/views/news.cshtml", out projectFileName));
            Assert.AreEqual("wwwroot\\views\\news.cshtml", projectFileName);

            Assert.IsFalse(pathMapperService.TryGetProjectFileName("/img/about.png", out projectFileName));
        }

        [Test]
        public void WebsiteItemPathToProjectFileNameTests()
        {
            var pathMapperService = new PathMapperService();

            pathMapperService.WebsiteItemPathToProjectDirectories.Add(new WebsiteItemPathToProjectDirectoryMapper("master", "/sitecore/content/Home", "/content/master/sitecore/content/Home", "item.json", "CleanBlog/**", string.Empty, string.Empty, string.Empty));

            string projectFileName;
            string format;
            Assert.IsTrue(pathMapperService.TryGetProjectFileName("/sitecore/content/Home/CleanBlog/Posts/Post1", string.Empty, out projectFileName, out format));
            Assert.AreEqual("content\\master\\sitecore\\content\\Home\\CleanBlog\\Posts\\Post1.item.json", projectFileName);
            Assert.AreEqual("item.json", format);

            Assert.IsFalse(pathMapperService.TryGetProjectFileName("/sitecore/content/Home/TodoMvc/Posts/Post1", string.Empty, out projectFileName, out format));
        }
    }
}
