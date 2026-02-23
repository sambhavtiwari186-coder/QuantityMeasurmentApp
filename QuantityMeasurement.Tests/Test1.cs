using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurement.Library.Model;

namespace QuantityMeasurement.Tests
{
    [TestClass]
    public class FeetTests
    {
        [TestMethod]
        public void GivenSameFeetValues_ShouldReturnTrue()
        {
            Feet first = new Feet(5.0);
            Feet second = new Feet(5.0);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void GivenDifferentFeetValues_ShouldReturnFalse()
        {
            Feet first = new Feet(5.0);
            Feet second = new Feet(6.0);

            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void GivenNullComparison_ShouldReturnFalse()
        {
            Feet first = new Feet(5.0);

            Assert.IsFalse(first.Equals(null));
        }
    }
}