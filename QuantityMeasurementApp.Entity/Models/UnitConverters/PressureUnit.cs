using System;

namespace QuantityMeasurementApp.Entity
{
    public class PressureUnit : IMeasurable
    {
        public static readonly PressureUnit Pascal = new PressureUnit("Pascal", 1.0);
        public static readonly PressureUnit Bar = new PressureUnit("Bar", 100000.0);
        public static readonly PressureUnit Psi = new PressureUnit("Psi", 6894.76);

        private readonly string name;
        private readonly double conversionFactor;

        private PressureUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Pressure";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "PASCAL": return Pascal;
                case "BAR": return Bar;
                case "PSI": return Psi;
                default: throw new ArgumentException($"Invalid Pressure unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
