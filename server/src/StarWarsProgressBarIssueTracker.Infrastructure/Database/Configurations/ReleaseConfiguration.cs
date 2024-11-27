using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable($"{nameof(Release)}s", IssueTrackerDbConfig.Schema);
        builder.HasIndex(release => release.GitlabId).IsUnique();
        builder.HasIndex(release => release.GitlabIid).IsUnique();
        builder.HasIndex(release => release.GitHubId).IsUnique();
        builder.Property(release => release.Title).IsRequired()
            .HasMaxLength(ReleaseConstants.MaxTitleLength);
        builder.Property(release => release.Notes).HasMaxLength(ReleaseConstants.MaxNotesLength);
        builder.HasMany(release => release.Issues).WithOne(issue => issue.Release).OnDelete(DeleteBehavior.SetNull);
    }
}
