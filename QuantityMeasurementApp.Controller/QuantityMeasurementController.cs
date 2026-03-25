using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Service;

namespace QuantityMeasurementApp.Controller
{
    [ApiController]
    [Route("api/measurement")]
    [Authorize]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityMeasurementService _service;
        private readonly IQuantityMeasurementRepository _repository;

        public QuantityMeasurementController(IQuantityMeasurementService service, IQuantityMeasurementRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        public class ConvertRequest
        {
            public QuantityDTO Source { get; set; } = null!;
            public string TargetUnit { get; set; } = string.Empty;
        }

        public class CompareRequest
        {
            public QuantityDTO Quantity1 { get; set; } = null!;
            public QuantityDTO Quantity2 { get; set; } = null!;
        }

        public class ArithmeticRequest
        {
            public QuantityDTO Quantity1 { get; set; } = null!;
            public QuantityDTO Quantity2 { get; set; } = null!;
            public string TargetUnit { get; set; } = string.Empty;
        }

        [HttpPost("convert")]
        public IActionResult Convert([FromBody] ConvertRequest request)
        {
            try
            {
                var result = _service.Convert(request.Source, request.TargetUnit);
                return Ok(result);
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("compare")]
        public IActionResult Compare([FromBody] CompareRequest request)
        {
            try
            {
                var result = _service.Compare(request.Quantity1, request.Quantity2);
                bool areEqual = result.Value == 1.0;
                return Ok(new { AreEqual = areEqual });
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] ArithmeticRequest request)
        {
            try
            {
                var result = _service.Add(request.Quantity1, request.Quantity2, request.TargetUnit);
                return Ok(result);
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("subtract")]
        public IActionResult Subtract([FromBody] ArithmeticRequest request)
        {
            try
            {
                var result = _service.Subtract(request.Quantity1, request.Quantity2, request.TargetUnit);
                return Ok(result);
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("history")]
        public ActionResult<List<QuantityMeasurementEntity>> GetHistory()
        {
            try
            {
                var results = _repository.GetAllMeasurements();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unable to retrieve history: {ex.Message}");
            }
        }
    }
}
