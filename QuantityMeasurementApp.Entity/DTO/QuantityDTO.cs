using System;

namespace QuantityMeasurementApp.Entity
{
    // DTO for holding quantity measurement input data
    public class QuantityDTO
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public string MeasurementType { get; set; }

        public QuantityDTO() { } // Required for Deserialization usually

        public QuantityDTO(double value, string unit, string measurementType)
        {
            Value = value;
            Unit = unit;
            MeasurementType = measurementType;
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}

