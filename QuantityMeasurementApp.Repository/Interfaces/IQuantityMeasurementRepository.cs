using System.Collections.Generic;
using QuantityMeasurementApp.Entity.Models;

namespace QuantityMeasurementApp.Repository.Interfaces
{
    // Interface Segregation Principle: specific interface for repository operations
    public interface IQuantityMeasurementRepository
    {
        // Save a new measurement entity to the repository
        void SaveMeasurement(QuantityMeasurementEntity measurement);

        // Retrieve all stored measurements
        IEnumerable<QuantityMeasurementEntity> GetAllMeasurements();
    }
}
