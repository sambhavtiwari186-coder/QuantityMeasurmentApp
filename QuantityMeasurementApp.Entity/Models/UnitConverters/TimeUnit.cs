using System;

namespace QuantityMeasurementApp.Entity
{
    public class TimeUnit : IMeasurable
    {
        public static readonly TimeUnit Second = new TimeUnit("Second", 1.0);
        public static readonly TimeUnit Minute = new TimeUnit("Minute", 60.0);
        public static readonly TimeUnit Hour = new TimeUnit("Hour", 3600.0);
        public static readonly TimeUnit Day = new TimeUnit("Day", 86400.0);
        public static readonly TimeUnit Week = new TimeUnit("Week", 604800.0);

        private readonly string name;
        private readonly double conversionFactor;

        private TimeUnit(string name, double conversionFactor)
        {
            this.name = name;
            this.conversionFactor = conversionFactor;
        }

        public double GetConversionFactor() => conversionFactor;
        public double ConvertToBaseUnit(double value) => value * conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => conversionFactor == 0 ? 0 : baseValue / conversionFactor;
        public string GetUnitName() => name;
        public string GetMeasurementType() => "Time";

        public IMeasurable GetUnitInstance(string name)
        {
            switch (name.ToUpper())
            {
                case "SECOND": return Second;
                case "MINUTE": return Minute;
                case "HOUR": return Hour;
                case "DAY": return Day;
                case "WEEK": return Week;
                default: throw new ArgumentException($"Invalid Time unit: {name}");
            }
        }

        public override string ToString() => name;
    }
}
