using System;

namespace QuantityMeasurement.Library.Model
{
    public sealed class Feet : IEquatable<Feet>
    {
        public double Value { get; }

        public Feet(double value)
        {
            Value = value;
        }

        public bool Equals(Feet other)
        {
            if (other is null)
                return false;

            return Value.CompareTo(other.Value) == 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Feet);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}