using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database;

public class IssueTrackerContext(DbContextOptions<IssueTrackerContext> options) : DbContext(options)
{
    public DbSet<Appearance> Appearances { get; init; } = default!;
    public DbSet<Issue> Issues { get; init; } = default!;
    public DbSet<IssueLink> IssueLinks { get; init; } = default!;
    public DbSet<Label> Labels { get; init; } = default!;
    public DbSet<Milestone> Milestones { get; init; } = default!;
    public DbSet<Release> Releases { get; init; } = default!;
    public DbSet<Vehicle> Vehicles { get; init; } = default!;
    public DbSet<Photo> Photos { get; init; } = default!;
    public DbSet<Translation> Translations { get; init; } = default!;
    public DbSet<DbJob> Jobs { get; init; } = default!;
    public DbSet<DbTask> Tasks { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IssueTrackerContext).Assembly);
    }

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
        var entries = ChangeTracker.Entries<DomainBase>();
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
