using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestClass]
    public class TextSpanTests : Tests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var position = new TextSpan(1, 2, 3);
            Assert.AreEqual(1, position.LineNumber);
            Assert.AreEqual(2, position.LinePosition);
            Assert.AreEqual(3, position.Length);
            Assert.AreEqual(472428, position.GetHashCode());
            Assert.IsFalse(position.Equals(null));

            var position2 = new TextSpan(1, 2, 3);
            Assert.AreEqual(position, position2);
            Assert.IsTrue(position == position2);
            Assert.IsTrue(position == position2);

            var position3 = new TextSpan(1, 2, 1);
            Assert.IsTrue(position != position3);
        }
    }
}