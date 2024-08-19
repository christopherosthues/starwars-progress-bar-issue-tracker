using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbIssueConfiguration : IEntityTypeConfiguration<DbIssue>
{
    public void Configure(EntityTypeBuilder<DbIssue> builder)
    {
        builder.ToTable("Issues", IssueTrackerDbConfig.Schema);
        builder.HasIndex(issue => issue.GitlabId).IsUnique();
        builder.HasIndex(issue => issue.GitlabIid).IsUnique();
        builder.HasIndex(issue => issue.GitHubId).IsUnique();
        builder.HasOne<DbVehicle>(issue => issue.Vehicle);
    }
}
