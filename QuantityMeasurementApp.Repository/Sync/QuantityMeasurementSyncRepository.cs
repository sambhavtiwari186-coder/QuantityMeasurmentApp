using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository.Sync
{
    public class QuantityMeasurementSyncRepository : IQuantityMeasurementRepository
    {
        private readonly IQuantityMeasurementRepository _database;
        private readonly PendingSyncStore _pendingStore;
        private readonly PendingSyncStore _historyStore;

        public QuantityMeasurementSyncRepository(
            IQuantityMeasurementRepository databaseRepository)
        {
            _database = databaseRepository;
            _pendingStore = new PendingSyncStore(System.IO.Path.Combine("Data", "measurement_pending.json"));
            _historyStore = new PendingSyncStore(System.IO.Path.Combine("Data", "measurement_history.json"));
        }

        public void SaveMeasurement(QuantityMeasurementEntity measurement)
        {
            if (measurement == null) throw new ArgumentNullException(nameof(measurement));

            try
            {
                // Try saving to DB directly
                _database.SaveMeasurement(measurement);
                
                // If it worked, save a backup in history.json
                _historyStore.Enqueue(measurement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SyncRepository] Database unavailable. Saving to pending.json: {ex.Message}");
                // Fallback to pending if DB is unavailable
                _pendingStore.Enqueue(measurement);
            }
        }

        public List<QuantityMeasurementEntity> GetAllMeasurements()
        {
            var allResults = new List<QuantityMeasurementEntity>();
            
            // 1. Try to get from Database
            try
            {
                var dbResults = _database.GetAllMeasurements();
                if (dbResults != null) allResults.AddRange(dbResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SyncRepository] Database read failed: {ex.Message}");
            }

            // 2. Always include history/pending store results as backups/unsynced data
            // We use a dictionary or HashSet to avoid showing duplicates if they are in both DB and local file
            var uniqueMeasurements = new Dictionary<int, QuantityMeasurementEntity>();
            
            // Add DB results first
            foreach (var item in allResults) 
            {
                if (item.Id != 0) uniqueMeasurements[item.Id] = item;
                else uniqueMeasurements[item.GetHashCode()] = item; // Fallback for unsaved items
            }

            // Add history store (backups)
            foreach (var item in _historyStore.LoadAll())
            {
                if (item.Id != 0 && !uniqueMeasurements.ContainsKey(item.Id)) uniqueMeasurements[item.Id] = item;
                // If it doesn't have an ID, it might be a duplicate, so we skip or add if unique enough
            }

            // Add pending store (unsynced)
            foreach (var item in _pendingStore.LoadAll())
            {
                // Unsynced items likely have ID 0, so we add them if they aren't already there by some other criteria
                // For simplicity in this fix, we just add them to the end of the final list
            }

            var finalizeList = uniqueMeasurements.Values.OrderByDescending(m => m.Timestamp).ToList();
            
            // Append pending at the top (most recent)
            var pending = _pendingStore.LoadAll().OrderByDescending(m => m.Timestamp).ToList();
            foreach (var p in pending)
            {
                // Check if already in finalizeList by some heuristic (e.g. timestamp and operand)
                if (!finalizeList.Any(f => f.Timestamp == p.Timestamp && f.FirstOperand == p.FirstOperand))
                {
                    finalizeList.Insert(0, p);
                }
            }

            return finalizeList;
        }

        public int GetTotalCount()
        {
            try
            {
                return _database.GetTotalCount();
            }
            catch (Exception)
            {
                return _historyStore.LoadAll().Count + _pendingStore.LoadAll().Count;
            }
        }

        public int SyncPendingToDatabase()
        {
            var pending = _pendingStore.LoadAll().ToList();
            if (pending.Count == 0) return 0;

            var remaining = new List<QuantityMeasurementEntity>();
            int synced = 0;

            foreach (var item in pending)
            {
                try
                {
                    _database.SaveMeasurement(item);
                    _historyStore.Enqueue(item); // Also back it up since it's now in DB
                    synced++;
                }
                catch
                {
                    remaining.Add(item);
                }
            }

            _pendingStore.OverwriteAll(remaining);
            if (synced > 0)
                Console.WriteLine($"[SyncRepository] Synced {synced} pending operation(s) to Database.");

            return synced;
        }
    }
}

