using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbIssueLinkConfiguration))]
public record DbIssueLink : DbEntityBase
{
    public LinkType Type { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public required DbIssue LinkedIssue { get; set; }
}
