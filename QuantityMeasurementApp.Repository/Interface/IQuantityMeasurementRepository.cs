using System.Collections.Generic;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    // Interface Segregation Principle: specific interface for repository operations
    public interface IQuantityMeasurementRepository
    {
        // Save a new measurement entity to the repository
        void SaveMeasurement(QuantityMeasurementEntity measurement);

        // Retrieve all stored measurements
        List<QuantityMeasurementEntity> GetAllMeasurements();

        // Return the total count of stored measurements
        int GetTotalCount();
    }
}

