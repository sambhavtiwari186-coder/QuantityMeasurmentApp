using System;

namespace QuantityMeasurementApp.Entity
{
    public class PowerUnit : IMeasurable
    {
        public static readonly PowerUnit Watt = new PowerUnit("Watt", 1.0);
        public static readonly PowerUnit Kilowatt = new PowerUnit("Kilowatt", 1000.0);
        public static readonly PowerUnit Horsepower = new PowerUnit("Horsepower", 745.7);

        private readonly string name;
        private readonly double conversionFactor;

        private PowerUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Power";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "WATT": return Watt;
                case "KILOWATT": return Kilowatt;
                case "HORSEPOWER": return Horsepower;
                default: throw new ArgumentException($"Invalid Power unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
