using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbTaskConfiguration))]
public class DbTask : DomainBase
{
    public required DbJob Job { get; set; }

    public TaskStatus Status { get; set; }

    public required DateTime ExecuteAt { get; set; }

    public DateTime? ExecutedAt { get; set; }
}
