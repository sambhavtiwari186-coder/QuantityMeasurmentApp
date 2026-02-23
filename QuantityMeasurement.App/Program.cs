using System;
using QuantityMeasurement.Library.Model;
using QuantityMeasurement.Library.Service;

namespace QuantityMeasurement.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Enter first value in feet: ");
                double value1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second value in feet: ");
                double value2 = Convert.ToDouble(Console.ReadLine());

                Feet feet1 = new Feet(value1);
                Feet feet2 = new Feet(value2);

                QuantityMeasurementService service = new QuantityMeasurementService();

                bool result = service.CompareFeet(feet1, feet2);

                Console.WriteLine($"Are the two values equal? {result}");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input. Please enter numeric values only.");
            }
        }
    }
}