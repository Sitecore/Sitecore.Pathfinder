// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;

namespace Sitecore.Pathfinder.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void EscapeXmlElementNameTests()
        {
            Assert.AreEqual("HelloWorld", "HelloWorld".EscapeXmlElementName());
            Assert.AreEqual("Hello.World", "Hello World".EscapeXmlElementName());
            Assert.AreEqual("Hello.World", "Hello.World".EscapeXmlElementName());
            Assert.AreEqual("Hello-World", "Hello-World".EscapeXmlElementName());
            Assert.AreEqual("_-1Hello.World", "1Hello World".EscapeXmlElementName());
            Assert.AreEqual("_-1Hello.World..", "1Hello World$ ".EscapeXmlElementName());
            Assert.AreEqual("__HelloWorld", "__HelloWorld".EscapeXmlElementName());
            Assert.AreEqual("Hello.-.World", "Hello - World".EscapeXmlElementName());
        }

        [Test]
        public void UnescapeXmlElementNameTests()
        {
            Assert.AreEqual("HelloWorld", "HelloWorld".UnescapeXmlElementName());
            Assert.AreEqual("Hello World", "Hello.World".UnescapeXmlElementName());
            Assert.AreEqual("Hello-World", "Hello-World".UnescapeXmlElementName());
            Assert.AreEqual("1Hello World", "_-1Hello.World".UnescapeXmlElementName());
            Assert.AreEqual("1Hello World  ", "_-1Hello.World..".UnescapeXmlElementName());
        }
    }
}
