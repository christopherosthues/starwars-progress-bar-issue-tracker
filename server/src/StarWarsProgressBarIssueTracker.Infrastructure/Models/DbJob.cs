using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbJobConfiguration))]
public record DbJob : DbEntityBase
{
    public required string CronInterval { get; set; }

    public bool IsPaused { get; set; }

    public DateTime? NextExecution { get; set; }

    public JobType JobType { get; set; }
}
