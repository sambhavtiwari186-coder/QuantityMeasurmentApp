using System;

namespace QuantityMeasurement.Library.Model
{
    public sealed class Length : IEquatable<Length>
    {
        private const double CM_TO_INCH = 0.393701;
        private const double FEET_TO_INCH = 12.0;
        private const double YARD_TO_INCH = 36.0;

        public double Value { get; }
        public Unit Unit { get; }

        public Length(double value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }

        private double ConvertToBaseUnit()
        {
            return Unit switch
            {
                Unit.Feet => Value * FEET_TO_INCH,
                Unit.Inch => Value,
                Unit.Yard => Value * YARD_TO_INCH,
                Unit.Centimeter => Value * CM_TO_INCH,
                _ => throw new InvalidOperationException("Unsupported unit")
            };
        }

        public bool Equals(Length other)
        {
            if (other is null)
                return false;

            double first = ConvertToBaseUnit();
            double second = other.ConvertToBaseUnit();

            return Math.Abs(first - second) < 0.0001;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Length);
        }

        public override int GetHashCode()
        {
            return ConvertToBaseUnit().GetHashCode();
        }
    }
}