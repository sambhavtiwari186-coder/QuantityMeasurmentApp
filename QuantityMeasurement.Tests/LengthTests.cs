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
        public void OneYard_Equals_ThreeFeet()
        {
            var yard = new Length(1, Unit.Yard);
            var feet = new Length(3, Unit.Feet);

            Assert.IsTrue(service.CompareLength(yard, feet));
        }

        [TestMethod]
        public void OneYard_Equals_ThirtySixInches()
        {
            var yard = new Length(1, Unit.Yard);
            var inch = new Length(36, Unit.Inch);

            Assert.IsTrue(service.CompareLength(yard, inch));
        }

        [TestMethod]
        public void TwoPointFiveFourCm_Equals_OneInch()
        {
            var cm = new Length(2.54, Unit.Centimeter);
            var inch = new Length(1, Unit.Inch);

            Assert.IsTrue(service.CompareLength(cm, inch));
        }

        [TestMethod]
        public void DifferentValues_ReturnFalse()
        {
            var yard = new Length(1, Unit.Yard);
            var feet = new Length(2, Unit.Feet);

            Assert.IsFalse(service.CompareLength(yard, feet));
        }
    }
}