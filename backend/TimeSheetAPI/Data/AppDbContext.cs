using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TimeSheet.Api.Models;
using TimeSheet.Api.Services.Interfaces;


namespace TimeSheet.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditEntities()
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditable && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntities)
            {
                var entity = (IAuditable)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedDate = DateTime.UtcNow;
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }
            }
        }
    }
}
