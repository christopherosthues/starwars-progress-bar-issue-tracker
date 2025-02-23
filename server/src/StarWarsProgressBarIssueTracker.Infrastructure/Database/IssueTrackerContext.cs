using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StarWarsProgressBarIssueTracker.CodeGen.EFCoreEnums;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Entities;
using TaskStatus = StarWarsProgressBarIssueTracker.Infrastructure.Entities.TaskStatus;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database;

[DatabaseEnumOptions(
    EntityNameFormat = "Db{0}",
    EnumNameMaxLength = IssueConstants.IssueStateMaxNameLength,
    EnumNamespace = "StarWarsProgressBarIssueTracker.Infrastructure.Entities",
    ConfigurationNamespace = "StarWarsProgressBarIssueTracker.Infrastructure.Configurations",
    SchemaName = IssueTrackerDbConfig.Schema)]
[DatabaseEnum(typeof(IssueState))]
[DatabaseEnum(typeof(Priority))]
[DatabaseEnum(typeof(LinkType))]
[DatabaseEnum(typeof(ReleaseState))]
[DatabaseEnum(typeof(EngineColor))]
[DatabaseEnum(typeof(JobType))]
[DatabaseEnum(typeof(TaskStatus))]
public class IssueTrackerContext(DbContextOptions<IssueTrackerContext> options) : DbContext(options)
{
    public DbSet<Appearance> Appearances { get; init; } = null!;
    public DbSet<Issue> Issues { get; init; } = null!;
    public DbSet<IssueLink> IssueLinks { get; init; } = null!;
    public DbSet<Label> Labels { get; init; } = null!;
    public DbSet<Milestone> Milestones { get; init; } = null!;
    public DbSet<Release> Releases { get; init; } = null!;
    public DbSet<Vehicle> Vehicles { get; init; } = null!;
    public DbSet<Photo> Photos { get; init; } = null!;
    public DbSet<Translation> Translations { get; init; } = null!;
    public DbSet<DbJob> Jobs { get; init; } = null!;
    public DbSet<DbTask> Tasks { get; init; } = null!;

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
        IEnumerable<EntityEntry<DomainBase>> entries = ChangeTracker.Entries<DomainBase>();
        foreach (EntityEntry<DomainBase> entry in entries)
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
