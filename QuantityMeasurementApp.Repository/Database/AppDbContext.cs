using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.Entities;

namespace QuantityMeasurementApp.Repository.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<QuantityMeasurementEntity> Measurements { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
