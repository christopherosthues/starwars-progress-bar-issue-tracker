using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Issues;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class IssueLinkConfiguration : IEntityTypeConfiguration<IssueLink>
{
    public void Configure(EntityTypeBuilder<IssueLink> builder)
    {
        builder.ToTable($"{nameof(IssueLink)}s", IssueTrackerDbConfig.Schema);
        builder.HasOne(issueLink => issueLink.LinkedIssue).WithMany(issue => issue.LinkedIssues)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
