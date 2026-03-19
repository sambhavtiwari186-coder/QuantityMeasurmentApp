using System;

namespace QuantityMeasurementApp.Entity
{
    // Generic POCO model class for representing a quantity internally
    public class QuantityModel<U> where U : IMeasurable
    {
        public double Value { get; }
        public U Unit { get; }

        public QuantityModel(double value, U unit)
        {
            if (!double.IsFinite(value))
            {
                throw new ArgumentException("Value must be a finite number.");
            }

            this.Value = value;
            this.Unit = unit;
        }

        public override string ToString()
        {
            return $"{Value} {Unit.GetUnitName()}";
        }
    }
}

