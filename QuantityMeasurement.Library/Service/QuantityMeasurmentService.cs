using QuantityMeasurement.Library.Model;
using System;

namespace QuantityMeasurement.Library.Service
{
    public class QuantityMeasurementService
    {
        public bool CompareLength(Length first, Length second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("Length cannot be null");

            return first.Equals(second);
        }
    }
}