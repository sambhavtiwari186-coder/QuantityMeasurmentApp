using System;
using QuantityMeasurement.Library.Model;
using QuantityMeasurement.Library.Service;

namespace QuantityMeasurement.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new QuantityMeasurementService();

            Console.Write("Enter first value: ");
            double value1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter unit (Feet/Inch/Yard/Centimeter): ");
            Unit unit1 = Enum.Parse<Unit>(Console.ReadLine(), true);

            Console.Write("Enter second value: ");
            double value2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter unit (Feet/Inch/Yard/Centimeter): ");
            Unit unit2 = Enum.Parse<Unit>(Console.ReadLine(), true);

            Length length1 = new Length(value1, unit1);
            Length length2 = new Length(value2, unit2);

            bool result = service.CompareLength(length1, length2);

            Console.WriteLine($"Equal? {result}");
        }
    }
}