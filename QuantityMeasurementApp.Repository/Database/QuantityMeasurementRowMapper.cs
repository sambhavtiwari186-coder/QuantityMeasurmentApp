using System;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    public static class QuantityMeasurementRowMapper
    {
        public static QuantityMeasurementEntity MapRow(SqlDataReader reader)
        {
            string firstOperand = reader.GetString(reader.GetOrdinal("FirstOperand"));
            string secondOperand = reader.GetString(reader.GetOrdinal("SecondOperand"));
            string operationType = reader.GetString(reader.GetOrdinal("OperationType"));
            string measurementType = reader.GetString(reader.GetOrdinal("MeasurementType"));
            string finalResult = reader.GetString(reader.GetOrdinal("FinalResult"));
            bool hasError = reader.GetBoolean(reader.GetOrdinal("HasError"));
            string errorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));
            DateTime recordedAt = reader.GetDateTime(reader.GetOrdinal("RecordedAt"));

            if (hasError)
            {
                // Preserve original stored error message and type.
                return new QuantityMeasurementEntity(
                    firstOperand,
                    secondOperand,
                    operationType,
                    measurementType,
                    "Error",
                    true,
                    errorMessage,
                    recordedAt);
            }

            if (string.Equals(secondOperand, "N/A", StringComparison.Ordinal))
                return new QuantityMeasurementEntity(firstOperand, operationType, finalResult, measurementType);

            return new QuantityMeasurementEntity(firstOperand, secondOperand, operationType, finalResult, measurementType);
        }
    }
}

