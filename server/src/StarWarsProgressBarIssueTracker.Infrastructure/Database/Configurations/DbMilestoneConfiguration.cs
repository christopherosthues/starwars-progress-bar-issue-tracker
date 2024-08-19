using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbMilestoneConfiguration : IEntityTypeConfiguration<DbMilestone>
{
    public void Configure(EntityTypeBuilder<DbMilestone> builder)
    {
        builder.ToTable("Milestones", IssueTrackerDbConfig.Schema);
        builder.HasIndex(milestone => milestone.GitlabId).IsUnique();
        builder.HasIndex(milestone => milestone.GitlabIid).IsUnique();
        builder.HasIndex(milestone => milestone.GitHubId).IsUnique();
    }
}
