// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;

namespace Sitecore.Pathfinder.Building
{
    [TestFixture]
    public class HelpWriterTests
    {
        [Test]
        public void WritePartialTests()
        {
            var helpWriter = new HelpWriter();

            helpWriter.Summary.Write("Summary");

            Assert.AreEqual("Summary", helpWriter.GetSummary());
            Assert.AreEqual("", helpWriter.GetExamples());
            Assert.AreEqual("None", helpWriter.GetParameters());
            Assert.AreEqual("Summary", helpWriter.GetRemarks());
        }

        [Test]
        public void WriteTests()
        {
            var helpWriter = new HelpWriter();

            helpWriter.Summary.Write("Summary");
            helpWriter.Examples.Write("Examples");
            helpWriter.Parameters.Write("Parameters");
            helpWriter.Remarks.Write("Remarks");

            Assert.AreEqual("Summary", helpWriter.GetSummary());
            Assert.AreEqual("Examples", helpWriter.GetExamples());
            Assert.AreEqual("Parameters", helpWriter.GetParameters());
            Assert.AreEqual("Remarks", helpWriter.GetRemarks());
        }
    }
}
