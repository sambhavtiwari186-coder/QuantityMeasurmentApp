using System;

namespace QuantityMeasurementApp.Entity.Models
{
    // Comprehensive data holder for storing operations in a repository
    [Serializable]
    public class QuantityMeasurementEntity
    {
        public string FirstOperand { get; }
        public string SecondOperand { get; }
        public string OperationType { get; }
        public string FinalResult { get; }
        public bool HasError { get; }
        public string ErrorMessage { get; }
        public DateTime Timestamp { get; }

        // Constructor for Single Operand Operations (e.g., Conversion)
        public QuantityMeasurementEntity(string operand, string operationType, string result)
        {
            this.FirstOperand = operand;
            this.SecondOperand = "N/A";
            this.OperationType = operationType;
            this.FinalResult = result;
            this.HasError = false;
            this.ErrorMessage = "None";
            this.Timestamp = DateTime.Now;
        }

        // Constructor for Binary Operand Operations (e.g., Addition, Comparison)
        public QuantityMeasurementEntity(string firstOperand, string secondOperand, string operationType, string result)
        {
            this.FirstOperand = firstOperand;
            this.SecondOperand = secondOperand;
            this.OperationType = operationType;
            this.FinalResult = result;
            this.HasError = false;
            this.ErrorMessage = "None";
            this.Timestamp = DateTime.Now;
        }

        // Constructor for Error scenarios
        public QuantityMeasurementEntity(string firstOperand, string secondOperand, string operationType, string errorMessage, bool hasError)
        {
            this.FirstOperand = firstOperand ?? "Unknown";
            this.SecondOperand = secondOperand ?? "Unknown";
            this.OperationType = operationType;
            this.FinalResult = "Error";
            this.HasError = hasError;
            this.ErrorMessage = errorMessage;
            this.Timestamp = DateTime.Now;
        }

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
