using QuantityMeasurementApp.Entity.Models;
using QuantityMeasurementApp.Service.Interfaces;

namespace QuantityMeasurementApp.Service.Interfaces
{
    // Interface for performing quantity operations
    public interface IQuantityMeasurementService
    {
        QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2);
        QuantityDTO Convert(QuantityDTO source, string targetUnitName);
        QuantityDTO Add(QuantityDTO q1, QuantityDTO q2, string targetUnitName);
        QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2, string targetUnitName);
        QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2);
    }
}
