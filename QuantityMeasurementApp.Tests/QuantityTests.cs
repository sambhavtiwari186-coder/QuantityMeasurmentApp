using System;
using System.Linq;
using NUnit.Framework;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Service;

namespace QuantityMeasurementApp.Tests
{
    public class QuantityTests
    {
        private IQuantityMeasurementService service;
        private IQuantityMeasurementRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = QuantityMeasurementCacheRepository.GetInstance();
            service = new QuantityMeasurementService(repository);
        }

        // --- Interface & Enum Setup Tests ---

        [Test]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            IMeasurable unit = LengthUnit.Feet;
            Assert.AreEqual("Feet", unit.GetUnitName());
            Assert.AreEqual("Length", unit.GetMeasurementType());
            Assert.AreEqual(1.0, unit.GetConversionFactor());
        }

        [Test]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            IMeasurable unit = WeightUnit.Kilogram;
            Assert.AreEqual("Kilogram", unit.GetUnitName());
            Assert.AreEqual("Weight", unit.GetMeasurementType());
            Assert.AreEqual(1.0, unit.GetConversionFactor());
        }

        // --- Service: Length Operations ---

        [Test]
        public void testService_LengthOperations_Equality()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");
            
            QuantityDTO result = service.Compare(first, second);
            Assert.AreEqual(1.0, result.Value);
        }

        [Test]
        public void testService_LengthOperations_Conversion()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO result = service.Convert(first, "Inch");
            
            Assert.AreEqual(12.0, result.Value);
            Assert.AreEqual("Inch", result.Unit);
        }

        [Test]
        public void testService_LengthOperations_Addition()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");
            
            QuantityDTO result = service.Add(first, second, "Feet");
            Assert.AreEqual(2.0, result.Value);
            Assert.AreEqual("Feet", result.Unit);
        }

        // --- Service: Weight Operations ---

        [Test]
        public void testService_WeightOperations_Equality()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(1000.0, "Gram", "Weight");
            
            QuantityDTO result = service.Compare(first, second);
            Assert.AreEqual(1.0, result.Value);
        }

        [Test]
        public void testService_WeightOperations_Addition()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(1000.0, "Gram", "Weight");
            
            QuantityDTO result = service.Add(first, second, "Kilogram");
            Assert.AreEqual(2.0, result.Value);
        }

        // --- Cross-Category & Validation ---

        [Test]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            QuantityDTO length = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO weight = new QuantityDTO(1.0, "Kilogram", "Weight");

            Assert.Throws<QuantityMeasurementException>(() => service.Compare(length, weight));
        }

        [Test]
        public void testService_ConstructorValidation_InvalidValue()
        {
            QuantityDTO invalid = new QuantityDTO(double.NaN, "Feet", "Length");
            Assert.Throws<QuantityMeasurementException>(() => service.Convert(invalid, "Inch"));
        }

        // --- Service: Volume Operations ---

        [Test]
        public void testService_Equality_LitreToLitre_SameValue()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Litre", "Volume");
            QuantityDTO second = new QuantityDTO(1.0, "Litre", "Volume");
            
            Assert.AreEqual(1.0, service.Compare(first, second).Value);
        }

        [Test]
        public void testService_Equality_LitreToGallon_EquivalentValue()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Litre", "Volume");
            QuantityDTO second = new QuantityDTO(0.264172, "Gallon", "Volume");
            
            Assert.AreEqual(1.0, service.Compare(first, second).Value);
        }

        // --- Service: Volume Addition Operations ---

        [Test]
        public void testService_Addition_CrossUnit_LitrePlusMillilitre()
        {
            QuantityDTO first = new QuantityDTO(1.0, "Litre", "Volume");
            QuantityDTO second = new QuantityDTO(1000.0, "Millilitre", "Volume");
            
            QuantityDTO result = service.Add(first, second, "Litre");
            Assert.AreEqual(2.0, result.Value);
        }

        // --- Generic Quantity: Subtraction Operations ---

        [Test]
        public void testService_Subtraction_CrossUnit_FeetMinusInches()
        {
            QuantityDTO first = new QuantityDTO(10.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(6.0, "Inch", "Length");
            
            QuantityDTO result = service.Subtract(first, second, "Feet");
            Assert.AreEqual(9.5, result.Value);
        }

        // --- Generic Quantity: Division Operations ---

        [Test]
        public void testService_Division_CrossUnit_FeetDividedByInches()
        {
            QuantityDTO first = new QuantityDTO(24.0, "Inch", "Length");
            QuantityDTO second = new QuantityDTO(2.0, "Feet", "Length");
            
            QuantityDTO result = service.Divide(first, second);
            Assert.AreEqual(1.0, result.Value);
        }

        [Test]
        public void testService_Division_ByZero()
        {
            QuantityDTO first = new QuantityDTO(10.0, "Feet", "Length");
            QuantityDTO zero = new QuantityDTO(0.0, "Feet", "Length");
            
            Assert.Throws<QuantityMeasurementException>(() => service.Divide(first, zero));
        }

        // --- Temperature Rejection Tests ---

        [Test]
        public void testService_Temperature_Addition_Rejection()
        {
            QuantityDTO t1 = new QuantityDTO(100.0, "Celsius", "Temperature");
            QuantityDTO t2 = new QuantityDTO(50.0, "Celsius", "Temperature");
            
            Assert.Throws<QuantityMeasurementException>(() => service.Add(t1, t2, "Celsius"));
        }

        [Test]
        public void testService_Temperature_Division_Rejection()
        {
            QuantityDTO t1 = new QuantityDTO(100.0, "Celsius", "Temperature");
            QuantityDTO t2 = new QuantityDTO(50.0, "Celsius", "Temperature");
            
            Assert.Throws<QuantityMeasurementException>(() => service.Divide(t1, t2));
        }

        // --- Repository Tracking Tests ---

        [Test]
        public void testRepository_TracksOperation()
        {
            // Count before
            int initialCount = repository.GetAllMeasurements().Count();

            QuantityDTO first = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");
            service.Add(first, second, "Feet");

            // Count after
            int finalCount = repository.GetAllMeasurements().Count();
            Assert.IsTrue(finalCount > initialCount);

            var latest = repository.GetAllMeasurements().Last();
            Assert.AreEqual("Addition", latest.OperationType);
            Assert.IsFalse(latest.HasError);
            Assert.IsTrue(latest.FinalResult.Contains("2 Feet"));
        }
    }
}