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
        public void Test000_Layout_Webconfig_File()
        {
            DoTest("Test000_Layout_Webconfig_File");
        }

        [Test]
        public void Test001_Layout_Renderings_Footerhtml_File()
        {
            DoTest("Test001_Layout_Renderings_Footerhtml_File");
        }

        [Test]
        public void Test002_Layout_Renderings_Htmltemplatehtml_File()
        {
            DoTest("Test002_Layout_Renderings_Htmltemplatehtml_File");
        }

        [Test]
        public void Test003_Sitecore_Templates_MyTemplate_Template()
        {
            DoTest("Test003_Sitecore_Templates_MyTemplate_Template");
        }

        [Test]
        public void Test004_Sitecore_Content_Home_HtmlTemplate_Item()
        {
            DoTest("Test004_Sitecore_Content_Home_HtmlTemplate_Item");
        }

        [Test]
        public void Test005_Sitecore_Layout_Renderings_Footer_Item()
        {
            DoTest("Test005_Sitecore_Layout_Renderings_Footer_Item");
        }

        [Test]
        public void Test006_Sitecore_Layout_Renderings_Htmltemplate_Item()
        {
            DoTest("Test006_Sitecore_Layout_Renderings_Htmltemplate_Item");
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
