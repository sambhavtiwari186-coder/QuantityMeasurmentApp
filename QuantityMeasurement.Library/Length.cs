using System;

namespace QuantityMeasurement.Library.Model
{
    public sealed class Length : IEquatable<Length>
    {
        public double Value { get; }
        public Unit Unit { get; }

        public Length(double value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }

        private double ConvertToBaseUnit()
        {
            // Base unit = Inch

            return Unit switch
            {
                Unit.Feet => Value * 12,
                Unit.Inch => Value,
                _ => throw new InvalidOperationException("Unsupported unit")
            };
        }

        public bool Equals(Length other)
        {
            if (other is null)
                return false;

            return ConvertToBaseUnit()
                .CompareTo(other.ConvertToBaseUnit()) == 0;
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