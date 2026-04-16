using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Service;
using QuantityMeasurementApp.Service.Interface;
using QuantityMeasurementApp.Entity.DTO;

namespace QuantityMeasurementApp.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var dto = new RegisterDTO 
            { 
                Email = request.Email, 
                Password = request.Password, 
                Username = request.Email 
            };
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return Conflict("User with this email already exists.");

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var dto = new LoginDTO 
            {
                Username = request.Email,
                Password = request.Password
            };
            var response = await _authService.LoginAsync(dto);
            if (response == null || string.IsNullOrEmpty(response.Token))
                return Unauthorized("Invalid credentials.");

            return Ok(new { token = response.Token });
        }
    }
}
