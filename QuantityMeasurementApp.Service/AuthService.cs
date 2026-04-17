using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.Entity.DTO;
using QuantityMeasurementApp.Entity.Entities;
using QuantityMeasurementApp.Repository.Database;
using QuantityMeasurementApp.Service.Interface;
using QuantityMeasurementApp.Service.Security;

namespace QuantityMeasurementApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (await _context.Set<UserEntity>().AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
            {
                throw new Exception("User already exists with this username or email.");
            }

            var user = new UserEntity
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashingHelper.HashPassword(dto.Password)
            };

            _context.Set<UserEntity>().Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
        
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Message = "Registration successful"
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Username == dto.Username);
            
            if (user == null || !HashingHelper.VerifyPassword(user.PasswordHash, dto.Password))
            {
                throw new Exception("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                Username = user.Username,
                Message = "Login successful"
            };
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "super_secret_key_that_is_long_enough_for_hmac_sha256");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
