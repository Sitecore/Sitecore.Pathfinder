// © 2015 Sitecore Corporation A/S. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.IO.PathMappers;

namespace Sitecore.Pathfinder.IO
{
    [TestClass]
    public class PathMapperTests : Tests
    {
        [TestMethod]
        public void ProjectFileNameToWebsiteFileNameTests()
        {
            var pathMapperService = new PathMapperService();
            var pathMatcher = new PathMatcher("**/*.html", "**/*.aspx");

            pathMapperService.ProjectDirectoryToWebsiteDirectories.Add(new ProjectDirectoryToWebsiteDirectoryMapper(pathMatcher, "/wwwroot", "/"));

            string websiteFileName;
            Assert.IsTrue(pathMapperService.TryGetWebsiteFileName("/wwwroot/about.html", out websiteFileName));
            Assert.AreEqual("~/about.html", websiteFileName);

            Assert.IsTrue(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.html", out websiteFileName));
            Assert.AreEqual("~/files/about.html", websiteFileName);

            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.aspx", out websiteFileName));
            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/wwwroot/files/about.ashx", out websiteFileName));
            Assert.IsFalse(pathMapperService.TryGetWebsiteFileName("/files/about.html", out websiteFileName));
        }

        [TestMethod]
        public void ProjectFileNameToWebsiteItemPathTests()
        {
            var pathMapperService = new PathMapperService();
            var pathMatcher = new PathMatcher("**/*.item.json", "**/*.aspx");

            pathMapperService.ProjectDirectoryToWebsiteItemPaths.Add(new ProjectDirectoryToWebsiteItemPathMapper(pathMatcher, "/content/master/sitecore", "master", "/sitecore", false, true));

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

        [TestMethod]
        public void WebsiteFileNameToProjectFileNameTests()
        {
            var pathMapperService = new PathMapperService();
            var pathMatcher = new PathMatcher("**/*.cshtml", "**/*.png");

            pathMapperService.WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper(pathMatcher, "/sitecore modules/shell/module", "/module"));
            pathMapperService.WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper(pathMatcher, "/", "/wwwroot"));

            string projectFileName;
            Assert.IsTrue(pathMapperService.TryGetProjectFileName("/sitecore modules/shell/module/overview.cshtml", out projectFileName));
            Assert.AreEqual("module\\overview.cshtml", projectFileName);

            Assert.IsTrue(pathMapperService.TryGetProjectFileName("/views/news.cshtml", out projectFileName));
            Assert.AreEqual("wwwroot\\views\\news.cshtml", projectFileName);

            Assert.IsFalse(pathMapperService.TryGetProjectFileName("/img/about.png", out projectFileName));
        }

        [TestMethod]
        public void WebsiteItemPathToProjectFileNameTests()
        {
            var pathMapperService = new PathMapperService();

            var itemNamePathMatcher = new PathMatcher("\\sitecore\\content\\Home\\CleanBlog\\**", string.Empty);

            var websiteItemPathToProjectDirectoryMapper = new WebsiteItemPathToProjectDirectoryMapper(itemNamePathMatcher, null, "master", "/sitecore/content/Home", "/content/master/sitecore/content/Home", "item.json");
            pathMapperService.WebsiteItemPathToProjectDirectories.Add(websiteItemPathToProjectDirectoryMapper);

            string projectFileName;
            string format;
            var condition = pathMapperService.TryGetProjectFileName("/sitecore/content/Home/CleanBlog/Posts/Post1", string.Empty, out projectFileName, out format);
            Assert.IsTrue(condition);
            Assert.AreEqual("content\\master\\sitecore\\content\\Home\\CleanBlog\\Posts\\Post1.item.json", projectFileName);
            Assert.AreEqual("item.json", format);

            Assert.IsFalse(pathMapperService.TryGetProjectFileName("/sitecore/content/Home/TodoMvc/Posts/Post1", string.Empty, out projectFileName, out format));
        }
    }
}
