using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.Entity.Models;
using QuantityMeasurementApp.Repository.Database;

namespace QuantityMeasurementApp.Service
{
    public interface IAuthService
    {
        Task<User?> RegisterUserAsync(string email, string password, string phoneNumber);
        Task<string?> LoginAsync(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> RegisterUserAsync(string email, string password, string phoneNumber)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null; // User already exists
            
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                PhoneNumber = phoneNumber,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid) return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "default_secret_key_which_should_be_long_enough";
            var keyChars = Encoding.UTF8.GetBytes(secretKey);
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyChars), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            
            return handler.WriteToken(token);
        }
    }
}
