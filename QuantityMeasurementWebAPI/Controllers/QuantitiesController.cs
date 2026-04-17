using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace QuantityMeasurementWebAPI.Controllers
{
    [ApiController]
    [Route("api/measurement")]
    [Authorize]
    public class QuantitiesController : ControllerBase
    {
        private readonly IQuantityMeasurementService _service;
        private readonly IQuantityMeasurementRepository _repository;
        private readonly IDistributedCache _cache;
        private const string HistoryCacheKey = "MeasurementHistory";

        public QuantitiesController(IQuantityMeasurementService service, IQuantityMeasurementRepository repository, IDistributedCache cache)
        {
            _service = service;
            _repository = repository;
            _cache = cache;
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "Ready", mode = "PostgreSQL" });
        }

        private void SafeRemoveCache(string key)
        {
            try { _cache.Remove(key); }
            catch (Exception ex) { Console.WriteLine($"Cache failure: {ex.Message}"); }
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
                SafeRemoveCache(HistoryCacheKey);
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
                SafeRemoveCache(HistoryCacheKey);
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
                SafeRemoveCache(HistoryCacheKey);
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
                SafeRemoveCache(HistoryCacheKey);
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

        [HttpPost("multiply")]
        public IActionResult Multiply([FromBody] ArithmeticRequest request)
        {
            try
            {
                var result = _service.Multiply(request.Quantity1, request.Quantity2, request.TargetUnit);
                SafeRemoveCache(HistoryCacheKey);
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

        [HttpPost("divide")]
        public IActionResult Divide([FromBody] ArithmeticRequest request)
        {
            try
            {
                // The backend Divide interface typically takes two args, returning Ratio
                var result = _service.Divide(request.Quantity1, request.Quantity2);
                SafeRemoveCache(HistoryCacheKey);
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
                try
                {
                    var cachedHistory = _cache.GetString(HistoryCacheKey);
                    if (!string.IsNullOrEmpty(cachedHistory))
                    {
                        var cachedResults = JsonSerializer.Deserialize<List<QuantityMeasurementEntity>>(cachedHistory);
                        return Ok(cachedResults);
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Cache read failure: {ex.Message}"); }

                var results = _repository.GetAllMeasurements();

                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };
                    _cache.SetString(HistoryCacheKey, JsonSerializer.Serialize(results), cacheOptions);
                }
                catch (Exception ex) { Console.WriteLine($"Cache write failure: {ex.Message}"); }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unable to retrieve history: {ex.Message}");
            }
        }
    }
}
