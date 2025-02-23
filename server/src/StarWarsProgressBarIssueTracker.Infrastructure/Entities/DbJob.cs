using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Entities;

[EntityTypeConfiguration(typeof(DbJobConfiguration))]
public class DbJob : DomainBase
{
    public required string CronInterval { get; set; }

    public bool IsPaused { get; set; }

    public DateTime? NextExecution { get; set; }

    public JobType JobType { get; set; }
}
