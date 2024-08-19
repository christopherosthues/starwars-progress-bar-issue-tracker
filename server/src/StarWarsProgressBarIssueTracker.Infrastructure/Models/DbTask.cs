using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbTasksConfiguration))]
public record DbTask : DbEntityBase
{
    public required DbJob Job { get; set; }

    public TaskStatus Status { get; set; }

    public required DateTime ExecuteAt { get; set; }

    public DateTime? ExecutedAt { get; set; }
}
