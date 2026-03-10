using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurement.Library.Model;
using QuantityMeasurement.Library.Service;

namespace QuantityMeasurement.Tests
{
    [TestClass]
    public class CrossUnitTests
    {
        private readonly QuantityMeasurementService service = new QuantityMeasurementService();

        [TestMethod]
        public void GivenOneFeetAndTwelveInch_ShouldReturnTrue()
        {
            Feet feet = new Feet(1);
            Inch inch = new Inch(12);

            Assert.IsTrue(service.CompareFeetAndInch(feet, inch));
        }

        [TestMethod]
        public void GivenTwoFeetAndTwentyFourInch_ShouldReturnTrue()
        {
            Feet feet = new Feet(2);
            Inch inch = new Inch(24);

            Assert.IsTrue(service.CompareFeetAndInch(feet, inch));
        }

        [TestMethod]
        public void GivenDifferentValues_ShouldReturnFalse()
        {
            Feet feet = new Feet(1);
            Inch inch = new Inch(10);

            Assert.IsFalse(service.CompareFeetAndInch(feet, inch));
        }
    }
}