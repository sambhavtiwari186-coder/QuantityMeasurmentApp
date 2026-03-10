using QuantityMeasurement.Library.Model;
using System;

namespace QuantityMeasurement.Library.Service
{
    public class QuantityMeasurementService
    {
        private const double FEET_TO_INCH = 12.0;

        public bool CompareFeet(Feet first, Feet second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("Feet values cannot be null");

            return first.Equals(second);
        }

        public bool CompareInch(Inch first, Inch second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("Inch values cannot be null");

            return first.Equals(second);
        }

        public bool CompareFeetAndInch(Feet feet, Inch inch)
        {
            if (feet == null || inch == null)
                throw new ArgumentNullException("Values cannot be null");

            double feetInInches = feet.Value * FEET_TO_INCH;

            return feetInInches.CompareTo(inch.Value) == 0;
        }
    }
}