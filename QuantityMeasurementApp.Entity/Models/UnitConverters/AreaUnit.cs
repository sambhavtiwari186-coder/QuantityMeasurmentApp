using System;

namespace QuantityMeasurementApp.Entity
{
    public class AreaUnit : IMeasurable
    {
        public static readonly AreaUnit SquareFoot = new AreaUnit("SquareFoot", 1.0);
        public static readonly AreaUnit SquareInch = new AreaUnit("SquareInch", 1.0 / 144.0);
        public static readonly AreaUnit SquareMeter = new AreaUnit("SquareMeter", 10.76391);
        public static readonly AreaUnit Acre = new AreaUnit("Acre", 43560.0);
        public static readonly AreaUnit Hectare = new AreaUnit("Hectare", 107639.104);

        private readonly string name;
        private readonly double conversionFactor;

        private AreaUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Area";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "SQUAREFOOT": return SquareFoot;
                case "SQUAREINCH": return SquareInch;
                case "SQUAREMETER": return SquareMeter;
                case "ACRE": return Acre;
                case "HECTARE": return Hectare;
                default: throw new ArgumentException($"Invalid Area unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
