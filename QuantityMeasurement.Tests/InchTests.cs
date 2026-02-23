using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurement.Library.Model;

namespace QuantityMeasurement.Tests
{
    [TestClass]
    public class InchTests
    {
        [TestMethod]
        public void GivenSameInchValues_ShouldReturnTrue()
        {
            Inch first = new Inch(12.0);
            Inch second = new Inch(12.0);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void GivenDifferentInchValues_ShouldReturnFalse()
        {
            Inch first = new Inch(12.0);
            Inch second = new Inch(15.0);

            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void GivenNullComparison_ShouldReturnFalse()
        {
            Inch first = new Inch(10.0);

            Assert.IsFalse(first.Equals(null));
        }
    }
}