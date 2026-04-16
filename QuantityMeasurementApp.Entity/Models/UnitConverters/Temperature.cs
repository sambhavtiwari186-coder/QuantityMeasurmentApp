using System;

namespace QuantityMeasurementApp.Entity
{
    public class TemperatureUnit : IMeasurable
    {
        // Base Unit: Celsius
        public static readonly TemperatureUnit Celsius = new TemperatureUnit("Celsius");
        public static readonly TemperatureUnit Fahrenheit = new TemperatureUnit("Fahrenheit");
        public static readonly TemperatureUnit Kelvin = new TemperatureUnit("Kelvin");

        private readonly string name;

        private TemperatureUnit(string name)
        {
            this.name = name;
        }

        // Dummy conversion factor since temperature uses non-linear functions
        public double GetConversionFactor() => 1.0;

        public double ConvertToBaseUnit(double value)
        {
            switch (name)
            {
                case "Celsius":
                    return value;
                case "Fahrenheit":
                    return (value - 32.0) * 5.0 / 9.0;
                case "Kelvin":
                    return value - 273.15;
                default:
                    throw new ArgumentException("Unknown temperature unit.");
            }
        }

        public double ConvertFromBaseUnit(double baseValue)
        {
            switch (name)
            {
                case "Celsius":
                    return baseValue;
                case "Fahrenheit":
                    return (baseValue * 9.0 / 5.0) + 32.0;
                case "Kelvin":
                    return baseValue + 273.15;
                default:
                    throw new ArgumentException("Unknown temperature unit.");
            }
        }

        public string GetUnitName() => name;

        public string GetMeasurementType() => "Temperature";

        public IMeasurable GetUnitInstance(string unitName)
        {
            string upperName = unitName.ToUpper();
            if (upperName == "CELSIUS")
            {
                return Celsius;
            }
            else if (upperName == "FAHRENHEIT")
            {
                return Fahrenheit;
            }
            else if (upperName == "KELVIN")
            {
                return Kelvin;
            }
            else
            {
                throw new ArgumentException($"Invalid Temperature unit: {unitName}");
            }
        }

        // UC14: Opt-out of Arithmetic Operations
        public bool SupportsArithmetic() => false;

        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException($"Temperature does not support {operation} operations.");
        }

        public override string ToString() => name;
    }
}

