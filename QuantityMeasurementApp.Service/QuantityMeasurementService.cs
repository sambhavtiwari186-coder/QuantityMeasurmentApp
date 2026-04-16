using System;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;

namespace QuantityMeasurementApp.Service
{
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository repository;

        public QuantityMeasurementService(IQuantityMeasurementRepository repository)
        {
            this.repository = repository;
        }

        // --- Helper Methods to Retrieve Typed Models from DTO ---
        
        private IMeasurable GetUnitFromDTO(QuantityDTO dto)
        {
            string measurementType = dto.MeasurementType.ToUpper();

            switch (measurementType)
            {
                case "LENGTH":
                    return LengthUnit.Feet.GetUnitInstance(dto.Unit);
                case "VOLUME":
                    return VolumeUnit.Litre.GetUnitInstance(dto.Unit);
                case "WEIGHT":
                    return WeightUnit.Kilogram.GetUnitInstance(dto.Unit);
                case "TEMPERATURE":
                    return TemperatureUnit.Celsius.GetUnitInstance(dto.Unit);
                case "AREA":
                    return AreaUnit.SquareFoot.GetUnitInstance(dto.Unit);
                case "SPEED":
                    return SpeedUnit.Kmph.GetUnitInstance(dto.Unit);
                case "TIME":
                    return TimeUnit.Second.GetUnitInstance(dto.Unit);
                case "ENERGY":
                    return EnergyUnit.Joule.GetUnitInstance(dto.Unit);
                case "PRESSURE":
                    return PressureUnit.Pascal.GetUnitInstance(dto.Unit);
                case "ANGLE":
                    return AngleUnit.Degree.GetUnitInstance(dto.Unit);
                case "POWER":
                    return PowerUnit.Watt.GetUnitInstance(dto.Unit);
                default:
                    throw new QuantityMeasurementException($"Unsupported Measurement Type: {dto.MeasurementType}");
            }
        }

        private QuantityModel<IMeasurable> GetModelFromDTO(QuantityDTO dto)
        {
            try
            {
                IMeasurable unit = GetUnitFromDTO(dto);
                return new QuantityModel<IMeasurable>(dto.Value, unit);
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Error extracting data from DTO: {ex.Message}", ex);
            }
        }

        private void EnsureSameMeasurementType(IMeasurable u1, IMeasurable u2)
        {
            if (u1.GetMeasurementType() != u2.GetMeasurementType())
            {
                throw new QuantityMeasurementException($"Cannot operate across different measurement types: {u1.GetMeasurementType()} vs {u2.GetMeasurementType()}");
            }
        }

        // --- Interface Implementations ---

        public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var model1 = GetModelFromDTO(q1);
                var model2 = GetModelFromDTO(q2);

                EnsureSameMeasurementType(model1.Unit, model2.Unit);

                double baseVal1 = Math.Round(model1.Unit.ConvertToBaseUnit(model1.Value), 5);
                double baseVal2 = Math.Round(model2.Unit.ConvertToBaseUnit(model2.Value), 5);
                
                bool areEqual = baseVal1.CompareTo(baseVal2) == 0;

                // Save Entity
                var resultString    = areEqual ? "Equal" : "Not Equal";
                var measurementType = q1.MeasurementType;
                var entity = new QuantityMeasurementEntity(
                    q1.ToString(),
                    q2.ToString(),
                    "Equality",
                    resultString,
                    measurementType);
                repository.SaveMeasurement(entity);

                // Return DTO containing boolean as string in the Value/Unit hack, 
                // but usually controllers handle bools natively. We'll return 1 for true, 0 for false.
                return new QuantityDTO(areEqual ? 1 : 0, "Boolean", "Result");
            }
            catch (Exception ex)
            {
                var entity = new QuantityMeasurementEntity(q1.ToString(), q2.ToString(), "Equality", ex.Message, true);
                repository.SaveMeasurement(entity);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
        }

        public QuantityDTO Convert(QuantityDTO source, string targetUnitName)
        {
            try
            {
                var model = GetModelFromDTO(source);
                IMeasurable targetUnit = model.Unit.GetUnitInstance(targetUnitName);

                double baseVal = model.Unit.ConvertToBaseUnit(model.Value);
                double convertedVal = Math.Round(targetUnit.ConvertFromBaseUnit(baseVal), 5);

                var resultDto = new QuantityDTO(
                    convertedVal,
                    targetUnit.GetUnitName(),
                    targetUnit.GetMeasurementType());

                var entity = new QuantityMeasurementEntity(
                    source.ToString(),
                    "Conversion",
                    resultDto.ToString(),
                    targetUnit.GetMeasurementType());
                repository.SaveMeasurement(entity);

                return resultDto;
            }
            catch (Exception ex)
            {
                var entity = new QuantityMeasurementEntity(source.ToString(), targetUnitName, "Conversion", ex.Message, true);
                repository.SaveMeasurement(entity);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
        }

        public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2, string targetUnitName)
        {
            return PerformArithmetic(q1, q2, targetUnitName, "Addition");
        }

        public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2, string targetUnitName)
        {
            return PerformArithmetic(q1, q2, targetUnitName, "Subtraction");
        }

        public QuantityDTO Multiply(QuantityDTO q1, QuantityDTO q2, string targetUnitName)
        {
            return PerformArithmetic(q1, q2, targetUnitName, "Multiplication");
        }

        public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var model1 = GetModelFromDTO(q1);
                var model2 = GetModelFromDTO(q2);

                model1.Unit.ValidateOperationSupport("Division");
                EnsureSameMeasurementType(model1.Unit, model2.Unit);

                double baseVal1 = model1.Unit.ConvertToBaseUnit(model1.Value);
                double baseVal2 = model2.Unit.ConvertToBaseUnit(model2.Value);

                if (Math.Abs(baseVal2) < 1e-10)
                {
                    throw new DivideByZeroException("Cannot divide by a quantity of zero.");
                }

                double result = baseVal1 / baseVal2;
                var resultDto = new QuantityDTO(result, "Ratio", "Scalar");

                var entity = new QuantityMeasurementEntity(
                    q1.ToString(),
                    q2.ToString(),
                    "Division",
                    resultDto.ToString(),
                    q1.MeasurementType);
                repository.SaveMeasurement(entity);

                return resultDto;
            }
            catch (Exception ex)
            {
                var entity = new QuantityMeasurementEntity(q1.ToString(), q2.ToString(), "Division", ex.Message, true);
                repository.SaveMeasurement(entity);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
        }

        private QuantityDTO PerformArithmetic(QuantityDTO q1, QuantityDTO q2, string targetUnitName, string operationName)
        {
            try
            {
                var model1 = GetModelFromDTO(q1);
                var model2 = GetModelFromDTO(q2);

                model1.Unit.ValidateOperationSupport(operationName);
                EnsureSameMeasurementType(model1.Unit, model2.Unit);

                IMeasurable targetUnit = model1.Unit.GetUnitInstance(targetUnitName);

                double baseVal1 = model1.Unit.ConvertToBaseUnit(model1.Value);
                double baseVal2 = model2.Unit.ConvertToBaseUnit(model2.Value);

                double baseResult = 0;
                if (operationName == "Addition")
                {
                    baseResult = baseVal1 + baseVal2;
                }
                else if (operationName == "Subtraction")
                {
                    baseResult = baseVal1 - baseVal2;
                }
                else if (operationName == "Multiplication")
                {
                    baseResult = baseVal1 * baseVal2;
                }
                else
                {
                    throw new QuantityMeasurementException("Unsupported Arithmetic Operation: " + operationName);
                }
                double targetResult = Math.Round(targetUnit.ConvertFromBaseUnit(baseResult), 5);

                var resultDto = new QuantityDTO(
                    targetResult,
                    targetUnit.GetUnitName(),
                    targetUnit.GetMeasurementType());

                var entity = new QuantityMeasurementEntity(
                    q1.ToString(),
                    q2.ToString(),
                    operationName,
                    resultDto.ToString(),
                    q1.MeasurementType);
                repository.SaveMeasurement(entity);

                return resultDto;
            }
            catch (Exception ex)
            {
                if (q1.MeasurementType.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                {
                    ex = new QuantityMeasurementException("Temperature does not support arithmetic operations.", ex);
                }
                var entity = new QuantityMeasurementEntity(q1.ToString(), q2.ToString(), operationName, ex.Message, true);
                repository.SaveMeasurement(entity);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
        }
    }
}
