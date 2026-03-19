using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Service;

namespace QuantityMeasurementApp.App
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString =
                "Data Source=.\\SQLEXPRESS;Database=QuantityMeasurementDB;Integrated Security=True;Persist Security Info=False;Pooling=False;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;" +
                "Application Name=\"SQL Server Management Studio\";Command Timeout=0";

            // ── Bootstrap the N-tier stack ──────────────────────────────────
            IQuantityMeasurementRepository repository;

            try
            {
                var cacheRepo = QuantityMeasurementCacheRepository.GetInstance();
                var dbRepo = new QuantityMeasurementDatabaseRepository(connectionString);
                var syncRepo = new QuantityMeasurementSyncRepository(cacheRepo, dbRepo);
                syncRepo.SyncPendingToDatabase();

                Console.WriteLine("[App] SQL Server available. Running in offline-first mode (cache -> sync).");
                repository = syncRepo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[App] SQL Server unavailable. Using cache-only mode. Reason: {ex.Message}");
                repository = QuantityMeasurementCacheRepository.GetInstance();
            }

            IQuantityMeasurementService   service    = new QuantityMeasurementService(repository);
            QuantityMeasurementController controller = new QuantityMeasurementController(service);

            // ── Hand off to the menu ────────────────────────────────────────
            IMenu menu = new Menu(controller);
            menu.Run();

            // ── Print summary on exit ───────────────────────────────────────
            int totalSaved = repository.GetTotalCount();
            Console.WriteLine($"\n[App] Session complete. Total measurements stored: {totalSaved}");
        }
    }
}