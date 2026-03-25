using System;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Salt { get; set; } = string.Empty;
        
        public string Role { get; set; } = "User";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
