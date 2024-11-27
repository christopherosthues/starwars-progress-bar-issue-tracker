using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.ToTable($"{nameof(Milestone)}s", IssueTrackerDbConfig.Schema);
        builder.HasIndex(milestone => milestone.GitlabId).IsUnique();
        builder.HasIndex(milestone => milestone.GitlabIid).IsUnique();
        builder.HasIndex(milestone => milestone.GitHubId).IsUnique();
        builder.Property(milestone => milestone.Title).IsRequired()
            .HasMaxLength(MilestoneConstants.MaxTitleLength);
        builder.Property(milestone => milestone.Description)
            .HasMaxLength(MilestoneConstants.MaxDescriptionLength);
        builder.HasMany(milestone => milestone.Issues).WithOne(issue => issue.Milestone)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
