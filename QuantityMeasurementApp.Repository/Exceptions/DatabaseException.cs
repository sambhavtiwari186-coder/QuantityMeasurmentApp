using System;

namespace QuantityMeasurementApp.Repository
{
    // UC16: Custom exception that wraps database-related errors so that upper
    // layers receive meaningful messages without needing a direct reference to
    // Microsoft.Data.SqlClient.SqlException.
    public class DatabaseException : Exception
    {
        public DatabaseException(string message)
            : base(message) { }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

