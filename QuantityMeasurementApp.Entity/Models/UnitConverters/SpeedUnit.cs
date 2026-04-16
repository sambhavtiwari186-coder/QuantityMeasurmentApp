using System;

namespace QuantityMeasurementApp.Entity
{
    public class SpeedUnit : IMeasurable
    {
        public static readonly SpeedUnit Kmph = new SpeedUnit("Kmph", 1.0);
        public static readonly SpeedUnit Mph = new SpeedUnit("Mph", 1.60934);
        public static readonly SpeedUnit Mps = new SpeedUnit("Mps", 3.6);

        private readonly string name;
        private readonly double conversionFactor;

        private SpeedUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Speed";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "KMPH": return Kmph;
                case "MPH": return Mph;
                case "MPS": return Mps;
                default: throw new ArgumentException($"Invalid Speed unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
