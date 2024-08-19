using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database;

public class IssueTrackerContext(DbContextOptions<IssueTrackerContext> options) : DbContext(options)
{
    public DbSet<DbAppearance> Appearances { get; init; } = default!;
    public DbSet<DbIssue> Issues { get; init; } = default!;
    public DbSet<DbIssueLink> IssueLinks { get; init; } = default!;
    public DbSet<DbLabel> Labels { get; init; } = default!;
    public DbSet<DbMilestone> Milestones { get; init; } = default!;
    public DbSet<DbRelease> Releases { get; init; } = default!;
    public DbSet<DbVehicle> Vehicles { get; init; } = default!;
    public DbSet<DbPhoto> Photos { get; init; } = default!;
    public DbSet<DbTranslation> Translations { get; init; } = default!;
    public DbSet<DbJob> Jobs { get; init; } = default!;
    public DbSet<DbTask> Tasks { get; init; } = default!;

    public override int SaveChanges()
    {
        UpdateAuditProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditProperties()
    {
        var entries = ChangeTracker.Entries<DbEntityBase>();
        foreach (var entry in entries)
        {
            DateTime utcNow = DateTime.UtcNow;
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = utcNow;
            }
        }
    }
}
