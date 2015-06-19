using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Documents.Xml;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Tests
{
    [TestFixture]
    public class PathfinderLocalTests
    {
        [Test]
        public void Test000_Sitecore_Content_Home_HelloWorld_Item()
        {
            DoTest("Test000_Sitecore_Content_Home_HelloWorld_Item");
        }

        [Test]
        public void Test001_Sitecore_Content_Home_Welcome_Item()
        {
            DoTest("Test001_Sitecore_Content_Home_Welcome_Item");
        }

        [Test]
        public void Test002_Sitecore_Layout_Renderings_HelloWorld_Item()
        {
            DoTest("Test002_Sitecore_Layout_Renderings_HelloWorld_Item");
        }

        [Test]
        public void Test003_Sitecore_MediaLibrary_Demo_Item()
        {
            DoTest("Test003_Sitecore_MediaLibrary_Demo_Item");
        }

        private void DoTest(string testName)
        {
            try
            {
                var output = new WebClient().DownloadString("http://Pathfinder/sitecore/shell/client/Applications/Pathfinder/Tests/WebTestRunner.ashx?test=" + HttpUtility.UrlEncode(testName));
                if (!string.IsNullOrEmpty(output))
                {
                    output = HttpUtility.HtmlDecode(output).Trim();
                }

                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine(output);
                }
            }
            catch (WebException ex)
            {
                var message = ex.Message;

                var stream = ex.Response?.GetResponseStream();
                if (stream != null)
                {
                    message = HttpUtility.HtmlDecode(new StreamReader(stream).ReadToEnd()) ?? string.Empty;
                }

                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
