using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    // UC16: ADO.NET-based repository that persists quantity measurements to
    // SQL Server via parameterised queries (SQL-injection safe).
    // Implements the full IQuantityMeasurementRepository contract.
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly string _connectionString;

        // ─── Constructor ──────────────────────────────────────────────────

        public QuantityMeasurementDatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
            Console.WriteLine("[DatabaseRepository] Initialized with SQL Server Connection.");
        }

        // ─── Private Helpers ──────────────────────────────────────────────

        private SqlConnection OpenConnection()
        {
            try
            {
                var conn = new SqlConnection(_connectionString);
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"Unable to open SQL Server connection: {ex.Message}", ex);
            }
        }

        // ─── IQuantityMeasurementRepository Implementation ────────────────

        /// <inheritdoc/>
        public void SaveMeasurement(QuantityMeasurementEntity measurement)
        {
            if (measurement == null) throw new ArgumentNullException(nameof(measurement));

            try
            {
                using var conn = OpenConnection();
                using var cmd  = new SqlCommand(SqlStatements.Insert, conn);

                cmd.Parameters.AddWithValue("@FirstOperand",    measurement.FirstOperand);
                cmd.Parameters.AddWithValue("@SecondOperand",   measurement.SecondOperand);
                cmd.Parameters.AddWithValue("@OperationType",   measurement.OperationType);
                cmd.Parameters.AddWithValue("@MeasurementType", measurement.MeasurementType);
                cmd.Parameters.AddWithValue("@FinalResult",     measurement.FinalResult);
                cmd.Parameters.AddWithValue("@HasError",        measurement.HasError);
                cmd.Parameters.AddWithValue("@ErrorMessage",    measurement.ErrorMessage);
                cmd.Parameters.AddWithValue("@RecordedAt",      measurement.Timestamp);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Error saving measurement: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public List<QuantityMeasurementEntity> GetAllMeasurements()
        {
            var results = new List<QuantityMeasurementEntity>();

            try
            {
                using var conn   = OpenConnection();
                using var cmd    = new SqlCommand(SqlStatements.SelectAll, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                    results.Add(QuantityMeasurementRowMapper.MapRow(reader));
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Error retrieving all measurements: {ex.Message}", ex);
            }

            return results;
        }

        /// <inheritdoc/>
        public int GetTotalCount()
        {
            try
            {
                using var conn = OpenConnection();
                using var cmd  = new SqlCommand(SqlStatements.Count, conn);
                return (int)cmd.ExecuteScalar()!;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Error getting measurement count: {ex.Message}", ex);
            }
        }
    }
}
