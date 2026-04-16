using System;
using System.Collections.Generic;
using System.Linq;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository.Database;

namespace QuantityMeasurementApp.Repository
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly AppDbContext _context;

        public QuantityMeasurementDatabaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public void SaveMeasurement(QuantityMeasurementEntity measurement)
        {
            if (measurement == null) throw new ArgumentNullException(nameof(measurement));

            try
            {
                _context.Measurements.Add(measurement);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error saving measurement: {ex.Message}", ex);
            }
        }

        public List<QuantityMeasurementEntity> GetAllMeasurements()
        {
            try
            {
                return _context.Measurements.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error retrieving all measurements: {ex.Message}", ex);
            }
        }

        public int GetTotalCount()
        {
            try
            {
                return _context.Measurements.Count();
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error getting measurement count: {ex.Message}", ex);
            }
        }
    }
}
