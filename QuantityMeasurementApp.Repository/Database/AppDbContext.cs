using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.Models;

namespace QuantityMeasurementApp.Repository.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<QuantityMeasurementEntity> Measurements { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
