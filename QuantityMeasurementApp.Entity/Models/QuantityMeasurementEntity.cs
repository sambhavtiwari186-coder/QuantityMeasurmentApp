using System;
using System.Text.Json.Serialization;

namespace QuantityMeasurementApp.Entity
{
    // Comprehensive data holder for storing operations in a repository
    [Serializable]
    public class QuantityMeasurementEntity
    {
        public string FirstOperand { get; }
        public string SecondOperand { get; }
        public string OperationType { get; }
        public string MeasurementType { get; }
        public string FinalResult { get; }
        public bool HasError { get; }
        public string ErrorMessage { get; }
        public DateTime Timestamp { get; }

        // Used by System.Text.Json deserialization (cache + pending sync files).
        [JsonConstructor]
        public QuantityMeasurementEntity(
            string firstOperand,
            string secondOperand,
            string operationType,
            string measurementType,
            string finalResult,
            bool hasError,
            string errorMessage,
            DateTime timestamp)
        {
            this.FirstOperand = firstOperand ?? "Unknown";
            this.SecondOperand = secondOperand ?? "Unknown";
            this.OperationType = operationType ?? "Unknown";
            this.MeasurementType = measurementType ?? "N/A";
            this.FinalResult = finalResult ?? "Error";
            this.HasError = hasError;
            this.ErrorMessage = errorMessage ?? "None";
            this.Timestamp = timestamp == default ? DateTime.Now : timestamp;
        }

        // Constructor for Single Operand Operations (e.g., Conversion)
        public QuantityMeasurementEntity(string operand, string operationType, string result, string measurementType = "N/A")
            : this(
                operand,
                "N/A",
                operationType,
                measurementType,
                result,
                false,
                "None",
                DateTime.Now)
        { }

        // Constructor for Binary Operand Operations (e.g., Addition, Comparison)
        public QuantityMeasurementEntity(string firstOperand, string secondOperand, string operationType, string result, string measurementType = "N/A")
            : this(
                firstOperand,
                secondOperand,
                operationType,
                measurementType,
                result,
                false,
                "None",
                DateTime.Now)
        { }

        // Constructor for Error scenarios
        public QuantityMeasurementEntity(string firstOperand, string secondOperand, string operationType, string errorMessage, bool hasError, string measurementType = "N/A")
            : this(
                firstOperand ?? "Unknown",
                secondOperand ?? "Unknown",
                operationType,
                measurementType,
                "Error",
                hasError,
                errorMessage,
                DateTime.Now)
        { }

        public override string ToString()
        {
            if (HasError)
            {
                return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] ERROR in {OperationType}: {ErrorMessage} (Operands: {FirstOperand}, {SecondOperand})";
            }
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {OperationType}: {FirstOperand} {(SecondOperand != "N/A" ? "and " + SecondOperand : "")} -> Result: {FinalResult}";
        }
    }
}

