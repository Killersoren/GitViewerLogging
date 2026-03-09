using Microsoft.EntityFrameworkCore;

namespace GitViewerLogging.DataAccess.Models
{
    public class LoggingContext : DbContext
    {
        public LoggingContext(DbContextOptions<LoggingContext> options) : base(options) { }

        public DbSet<LogEntity> LogEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TimeStamp).IsRequired();
                entity.Property(e => e.IsAnonymous).IsRequired();
                entity.Property(e => e.UserId).IsRequired(false);
                entity.Property(e => e.EntityName).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).IsRequired(false);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Details).IsRequired(false).HasMaxLength(4000);
            });
        }
    }
}
