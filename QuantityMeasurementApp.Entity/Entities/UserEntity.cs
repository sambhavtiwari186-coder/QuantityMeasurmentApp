using System;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.Entities
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
