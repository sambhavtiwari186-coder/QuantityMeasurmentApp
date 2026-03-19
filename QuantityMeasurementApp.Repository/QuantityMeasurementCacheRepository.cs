using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    // Singleton Cache Repository implementing IQuantityMeasurementRepository
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static QuantityMeasurementCacheRepository? instance;
        private static readonly object lockObject = new object();

        private readonly List<QuantityMeasurementEntity> measurementCache;
        private readonly string filePath = "measurement_history.json";

        // Private constructor for Singleton pattern
        private QuantityMeasurementCacheRepository()
        {
            measurementCache = new List<QuantityMeasurementEntity>();
            LoadFromDisk();
        }

        // Global access point for the Singleton instance
        public static QuantityMeasurementCacheRepository GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new QuantityMeasurementCacheRepository();
                    }
                }
            }
            return instance;
        }

        public void SaveMeasurement(QuantityMeasurementEntity measurement)
        {
            lock (lockObject)
            {
                measurementCache.Add(measurement);
                SaveToDisk();
            }
        }

        public List<QuantityMeasurementEntity> GetAllMeasurements()
        {
            lock (lockObject)
            {
                // Return a copy to prevent external modification
                return new List<QuantityMeasurementEntity>(measurementCache);
            }
        }

        public int GetTotalCount()
        {
            lock (lockObject)
            {
                return measurementCache.Count;
            }
        }

        // Persistence: Save to disk using JSON
        private void SaveToDisk()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };
                string jsonString = JsonSerializer.Serialize(measurementCache, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving cache to disk: {ex.Message}");
            }
        }

        // Persistence: Load from disk
        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var loadedMeasurements = JsonSerializer.Deserialize<List<QuantityMeasurementEntity>>(jsonString, options);
                    if (loadedMeasurements != null)
                    {
                        measurementCache.AddRange(loadedMeasurements);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cache from disk: {ex.Message}");
            }
        }
    }
}
