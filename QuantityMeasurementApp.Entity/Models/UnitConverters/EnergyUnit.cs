using System;

namespace QuantityMeasurementApp.Entity
{
    public class EnergyUnit : IMeasurable
    {
        public static readonly EnergyUnit Joule = new EnergyUnit("Joule", 1.0);
        public static readonly EnergyUnit Kilojoule = new EnergyUnit("Kilojoule", 1000.0);
        public static readonly EnergyUnit Megajoule = new EnergyUnit("Megajoule", 1000000.0);
        public static readonly EnergyUnit Gigajoule = new EnergyUnit("Gigajoule", 1000000000.0);
        public static readonly EnergyUnit Watthour = new EnergyUnit("Watthour", 3600.0);
        public static readonly EnergyUnit Calorie = new EnergyUnit("Calorie", 4.184);
        public static readonly EnergyUnit Kilocalorie = new EnergyUnit("Kilocalorie", 4184.0);

        private readonly string name;
        private readonly double conversionFactor;

        private EnergyUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Energy";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "JOULE": return Joule;
                case "KILOJOULE": return Kilojoule;
                case "MEGAJOULE": return Megajoule;
                case "GIGAJOULE": return Gigajoule;
                case "WATTHOUR": return Watthour;
                case "CALORIE": return Calorie;
                case "KILOCALORIE": return Kilocalorie;
                default: throw new ArgumentException($"Invalid Energy unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
