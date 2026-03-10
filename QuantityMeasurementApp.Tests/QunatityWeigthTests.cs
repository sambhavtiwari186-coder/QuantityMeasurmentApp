using System;
using NUnit.Framework;
using QuantityMeasurementApp.Core;

namespace QuantityMeasurementApp.Tests
{
    public class QuantityWeightTests
    {
        [Test]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(2.0, WeightUnit.Kilogram);
            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            QuantityWeight first = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_WeightVsLength_Incompatible()
        {
            QuantityWeight weight = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityLength length = new QuantityLength(1.0, LengthUnit.Feet);
            Assert.IsFalse(weight.Equals(length));
        }

        [Test]
        public void testEquality_NullComparison()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.IsFalse(first.Equals(null));
        }

        [Test]
        public void testEquality_SameReference()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.IsTrue(first.Equals(first));
        }

        [Test]
        public void testEquality_TransitiveProperty()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight third = new QuantityWeight(2.20462, WeightUnit.Pound);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(third));
            Assert.IsTrue(first.Equals(third));
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            QuantityWeight first = new QuantityWeight(0.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(0.0, WeightUnit.Gram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_NegativeWeight()
        {
            QuantityWeight first = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(-1000.0, WeightUnit.Gram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_LargeWeightValue()
        {
            QuantityWeight first = new QuantityWeight(1000000.0, WeightUnit.Gram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Kilogram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testEquality_SmallWeightValue()
        {
            QuantityWeight first = new QuantityWeight(0.001, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Gram);
            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void testConversion_PoundToKilogram()
        {
            QuantityWeight first = new QuantityWeight(2.20462, WeightUnit.Pound);
            Assert.AreEqual(1.0, first.ConvertTo(WeightUnit.Kilogram), 0.0001);
        }

        [Test]
        public void testConversion_KilogramToPound()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            Assert.AreEqual(2.20462, first.ConvertTo(WeightUnit.Pound), 0.0001);
        }

        [Test]
        public void testConversion_SameUnit()
        {
            QuantityWeight first = new QuantityWeight(5.0, WeightUnit.Kilogram);
            Assert.AreEqual(5.0, first.ConvertTo(WeightUnit.Kilogram));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            QuantityWeight first = new QuantityWeight(0.0, WeightUnit.Kilogram);
            Assert.AreEqual(0.0, first.ConvertTo(WeightUnit.Gram));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            QuantityWeight first = new QuantityWeight(-1.0, WeightUnit.Kilogram);
            Assert.AreEqual(-1000.0, first.ConvertTo(WeightUnit.Gram));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            QuantityWeight first = new QuantityWeight(1.5, WeightUnit.Kilogram);
            double grams = first.ConvertTo(WeightUnit.Gram);
            QuantityWeight mid = new QuantityWeight(grams, WeightUnit.Gram);
            
            Assert.AreEqual(1.5, mid.ConvertTo(WeightUnit.Kilogram), 0.0001);
        }

        [Test]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(2.0, WeightUnit.Kilogram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(3.0, WeightUnit.Kilogram)));
        }

        [Test]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(2.0, WeightUnit.Kilogram)));
        }

        [Test]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            QuantityWeight first = new QuantityWeight(2.20462, WeightUnit.Pound);
            QuantityWeight second = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(4.40924, WeightUnit.Pound)));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Gram()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            QuantityWeight result = first.Add(second, WeightUnit.Gram);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(2000.0, WeightUnit.Gram)));
        }

        [Test]
        public void testAddition_Commutativity()
        {
            QuantityWeight first = new QuantityWeight(1.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000.0, WeightUnit.Gram);
            
            QuantityWeight result1 = first.Add(second, WeightUnit.Kilogram);
            QuantityWeight result2 = second.Add(first, WeightUnit.Kilogram);
            
            Assert.IsTrue(result1.Equals(result2));
        }

        [Test]
        public void testAddition_WithZero()
        {
            QuantityWeight first = new QuantityWeight(5.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(0.0, WeightUnit.Gram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(5.0, WeightUnit.Kilogram)));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            QuantityWeight first = new QuantityWeight(5.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(-2000.0, WeightUnit.Gram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(3.0, WeightUnit.Kilogram)));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            QuantityWeight first = new QuantityWeight(1000000.0, WeightUnit.Kilogram);
            QuantityWeight second = new QuantityWeight(1000000.0, WeightUnit.Kilogram);
            QuantityWeight result = first.Add(second);
            
            Assert.IsTrue(result.Equals(new QuantityWeight(2000000.0, WeightUnit.Kilogram)));
        }
    }
}