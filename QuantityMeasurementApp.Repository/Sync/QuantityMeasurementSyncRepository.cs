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
            _pendingStore = new PendingSyncStore("measurement_pending.json");
            _historyStore = new PendingSyncStore("measurement_history.json");
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
            try
            {
                return _database.GetAllMeasurements();
            }
            catch (Exception)
            {
                // Fallback to reading from history and pending if DB is down
                var all = _historyStore.LoadAll().ToList();
                all.AddRange(_pendingStore.LoadAll());
                return all;
            }
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

