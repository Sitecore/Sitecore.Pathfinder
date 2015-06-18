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
        public void Test0_sitecorecontentHomeHelloWorld()
        {
            DoTest("Test0_sitecorecontentHomeHelloWorld");
        }

        [Test]
        public void Test1_sitecorecontentHomeHola()
        {
            DoTest("Test1_sitecorecontentHomeHola");
        }

        [Test]
        public void Test2_sitecorecontentHomeWelcome()
        {
            DoTest("Test2_sitecorecontentHomeWelcome");
        }

        [Test]
        public void Test3_sitecorelayoutrenderingsHelloWorld()
        {
            DoTest("Test3_sitecorelayoutrenderingsHelloWorld");
        }

        [Test]
        public void Test4_sitecoremediaLibrarydemo()
        {
            DoTest("Test4_sitecoremediaLibrarydemo");
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
