using System;

namespace QuantityMeasurementApp.Service.Services
{
    // Custom exception to handle errors and exceptional conditions for quantities
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException(string message) : base(message)
        {
        }

        public QuantityMeasurementException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
