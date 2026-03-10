using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurement.Library.Model;
using QuantityMeasurement.Library.Service;

namespace QuantityMeasurement.Tests
{
    [TestClass]
    public class LengthTests
    {
        private readonly QuantityMeasurementService service =
            new QuantityMeasurementService();

        [TestMethod]
        public void GivenOneFeetAndTwelveInch_ShouldReturnTrue()
        {
            Length feet = new Length(1, Unit.Feet);
            Length inch = new Length(12, Unit.Inch);

            Assert.IsTrue(service.CompareLength(feet, inch));
        }

        [TestMethod]
        public void GivenTwoFeetAndTwentyFourInch_ShouldReturnTrue()
        {
            Length feet = new Length(2, Unit.Feet);
            Length inch = new Length(24, Unit.Inch);

            Assert.IsTrue(service.CompareLength(feet, inch));
        }

        [TestMethod]
        public void GivenDifferentValues_ShouldReturnFalse()
        {
            Length feet = new Length(1, Unit.Feet);
            Length inch = new Length(10, Unit.Inch);

            Assert.IsFalse(service.CompareLength(feet, inch));
        }
    }
}