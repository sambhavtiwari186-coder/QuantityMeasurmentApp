using System;

namespace QuantityMeasurementApp.Entity
{
    public class AngleUnit : IMeasurable
    {
        public static readonly AngleUnit Degree = new AngleUnit("Degree", 1.0);
        public static readonly AngleUnit Radian = new AngleUnit("Radian", 180.0 / Math.PI);
        public static readonly AngleUnit Gradian = new AngleUnit("Gradian", 0.9);

        private readonly string name;
        private readonly double conversionFactor;

        private AngleUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Angle";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "DEGREE": return Degree;
                case "RADIAN": return Radian;
                case "GRADIAN": return Gradian;
                default: throw new ArgumentException($"Invalid Angle unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
