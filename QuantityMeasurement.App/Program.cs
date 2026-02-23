using System;
using QuantityMeasurement.Library.Model;
using QuantityMeasurement.Library.Service;

namespace QuantityMeasurement.App
{
    class Program
    {
        static void Main(string[] args)
        {
            QuantityMeasurementService service = new QuantityMeasurementService();

            Console.WriteLine("1. Compare Feet");
            Console.WriteLine("2. Compare Inch");
            Console.Write("Select option: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            try
            {
                Console.Write("Enter first value: ");
                double value1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());

                if (choice == 1)
                {
                    Feet f1 = new Feet(value1);
                    Feet f2 = new Feet(value2);

                    Console.WriteLine($"Equal? {service.CompareFeet(f1, f2)}");
                }
                else if (choice == 2)
                {
                    Inch i1 = new Inch(value1);
                    Inch i2 = new Inch(value2);

                    Console.WriteLine($"Equal? {service.CompareInch(i1, i2)}");
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
            catch
            {
                Console.WriteLine("Invalid numeric input.");
            }
        }
    }
}