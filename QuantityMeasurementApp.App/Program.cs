using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Repository.Interfaces;
using QuantityMeasurementApp.Repository.Services;
using QuantityMeasurementApp.Service.Interfaces;
using QuantityMeasurementApp.Service.Services;
using QuantityMeasurementApp.App.Interfaces;
using QuantityMeasurementApp.App.Services;

namespace QuantityMeasurementApp.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // ── Bootstrap the N-tier stack ──────────────────────────────────
            IQuantityMeasurementRepository repository = QuantityMeasurementCacheRepository.GetInstance();
            IQuantityMeasurementService     service    = new QuantityMeasurementService(repository);
            QuantityMeasurementController   controller = new QuantityMeasurementController(service);

            // ── Hand off to the menu ────────────────────────────────────────
            IMenu menu = new Menu(controller);
            menu.Run();
        }
    }
}