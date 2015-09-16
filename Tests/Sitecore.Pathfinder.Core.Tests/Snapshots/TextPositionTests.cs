using System;
using System.IO;
using NUnit.Framework;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestFixture]
    public class TextPositionTests : Tests
    {
        [Test]
        public void ConstructorTest()
        {
            var position = new TextPosition(1, 2, 3);
            Assert.AreEqual(1, position.LineNumber);
            Assert.AreEqual(2, position.LinePosition);
            Assert.AreEqual(3, position.LineLength);
            Assert.AreEqual(472428, position.GetHashCode());
            Assert.IsFalse(position.Equals(null));

            var position2 = new TextPosition(1, 2, 3);
            Assert.AreEqual(position, position2);
            Assert.IsTrue(position == position2);
            Assert.IsTrue(position == position2);

            var position3 = new TextPosition(1, 2, 1);
            Assert.IsTrue(position != position3);
        }
    }
}