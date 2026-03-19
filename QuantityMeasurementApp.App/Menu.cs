using System;
using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.App
{
    public class Menu : IMenu
    {
        private readonly QuantityMeasurementController _controller;

        public Menu(QuantityMeasurementController controller)
        {
            _controller = controller;
        }

        public void Run()
        {
            while (true)
            {
                string[] operations = new string[]
                {
                    "Compare two quantities",
                    "Convert a quantity",
                    "Add two quantities",
                    "Subtract two quantities",
                    "Divide two quantities",
                    "Exit"
                };

                int opChoice = MenuHelpers.PickFromMenu("Select Operation", operations);

                if (opChoice == 6)
                {
                    Console.WriteLine("Goodbye.");
                    return;
                }

                
                string[] allKeys = new string[] { "Length", "Volume", "Weight", "Temperature" };
                string[] allowedTypes;

                if (opChoice == 1 || opChoice == 2)
                {
                    allowedTypes = allKeys;
                }
                else
                {
                    // Arithmetic operations don't support Temperature
                    allowedTypes = new string[] { "Length", "Volume", "Weight" };
                }

                string measurementType = MenuHelpers.PickMeasurementType(allowedTypes);
                string[] available = MenuHelpers.Units[measurementType];

                switch (opChoice)
                {
                    case 1:
                        _controller.PerformComparison(
                            MenuHelpers.ReadQuantity("1st Quantity", measurementType, available),
                            MenuHelpers.ReadQuantity("2nd Quantity", measurementType, available));
                        break;

                    case 2:
                        _controller.PerformConversion(
                            MenuHelpers.ReadQuantity("Source", measurementType, available),
                            MenuHelpers.PickUnit("Convert To", available));
                        break;

                    case 3:
                        _controller.PerformAddition(
                            MenuHelpers.ReadQuantity("1st Quantity", measurementType, available),
                            MenuHelpers.ReadQuantity("2nd Quantity", measurementType, available),
                            MenuHelpers.PickUnit("Result Unit", available));
                        break;

                    case 4:
                        _controller.PerformSubtraction(
                            MenuHelpers.ReadQuantity("1st Quantity", measurementType, available),
                            MenuHelpers.ReadQuantity("2nd Quantity", measurementType, available),
                            MenuHelpers.PickUnit("Result Unit", available));
                        break;

                    case 5:
                        _controller.PerformDivision(
                            MenuHelpers.ReadQuantity("Numerator", measurementType, available),
                            MenuHelpers.ReadQuantity("Denominator", measurementType, available));
                        break;
                }

                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
