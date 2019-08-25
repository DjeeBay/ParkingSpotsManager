using Microsoft.EntityFrameworkCore;
using ParkingSpotsManager.Shared.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ParkingSpotsManager.Shared.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace ParkingSpotsManager.Shared.Database
{
    public class DataContext : DbContext
    {
        public int UserId { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<UserParking> UsersParkings { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Spot>().HasQueryFilter(s => !s.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var test = UserId;
            var newEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added && x.Entity != null && typeof(IWithTimestamps).IsAssignableFrom(x.Entity.GetType()))
                .Select(x => x.Entity as IWithTimestamps);
            var updatedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified && x.Entity != null && typeof(IWithTimestamps).IsAssignableFrom(x.Entity.GetType()))
                .Select(x => x.Entity as IWithTimestamps);

            foreach (var entity in newEntities) {
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = UserId;
            }
            foreach (var entity in updatedEntities) {
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = UserId;
            }

            return await base.SaveChangesAsync();
        }
    }
}
