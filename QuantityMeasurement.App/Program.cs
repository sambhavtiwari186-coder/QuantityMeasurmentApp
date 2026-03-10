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
Console.WriteLine("3. Compare Feet & Inch");
Console.Write("Select option: ");

int choice = Convert.ToInt32(Console.ReadLine());

try
{
    if (choice == 3)
    {
        Console.Write("Enter value in Feet: ");
        double feetValue = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter value in Inch: ");
        double inchValue = Convert.ToDouble(Console.ReadLine());

        Feet feet = new Feet(feetValue);
        Inch inch = new Inch(inchValue);

        Console.WriteLine($"Equal? {service.CompareFeetAndInch(feet, inch)}");
    }
}
catch
{
    Console.WriteLine("Invalid input.");
}
        }
    }
}