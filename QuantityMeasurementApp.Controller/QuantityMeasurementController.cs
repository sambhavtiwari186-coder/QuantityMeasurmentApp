using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Service;

namespace QuantityMeasurementApp.Controller
{
    // Facade controller that orchestrates User requests and Service logic
    public class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            this.service = service;
        }

        public void PerformComparison(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                QuantityDTO result = service.Compare(q1, q2);
                bool areEqual = result.Value == 1.0;
                Console.WriteLine($"Comparison: {q1} equals {q2} -> Output: {areEqual}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error during Comparison: {ex.Message}");
            }
            catch (Exception ex) // Catch-all for unexpected issues
            {
                Console.WriteLine($"Unexpected error during Comparison: {ex.Message}");
            }
        }

        public void PerformConversion(QuantityDTO source, string targetUnitName)
        {
            try
            {
                QuantityDTO result = service.Convert(source, targetUnitName);
                Console.WriteLine($"Converted: Quantity({source.Value}, {source.Unit}).ConvertTo({targetUnitName}) -> Output: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error during Conversion: {ex.Message}");
            }
        }

        public void PerformAddition(QuantityDTO q1, QuantityDTO q2, string targetUnitName)
        {
            try
            {
                QuantityDTO result = service.Add(q1, q2, targetUnitName);
                Console.WriteLine($"Addition: {q1} + {q2} in {targetUnitName} -> Output: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error Catch: {ex.Message}");
            }
        }

        public void PerformSubtraction(QuantityDTO q1, QuantityDTO q2, string targetUnitName)
        {
            try
            {
                QuantityDTO result = service.Subtract(q1, q2, targetUnitName);
                Console.WriteLine($"Subtraction: {q1} - {q2} in {targetUnitName} -> Output: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error Catch: {ex.Message}");
            }
        }

        public void PerformDivision(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                QuantityDTO result = service.Divide(q1, q2);
                Console.WriteLine($"Division: {q1} / {q2} -> Output: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error Catch: {ex.Message}");
            }
        }
    }
}
