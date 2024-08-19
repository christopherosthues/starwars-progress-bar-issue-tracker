using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbReleaseConfiguration : IEntityTypeConfiguration<DbRelease>
{
    public void Configure(EntityTypeBuilder<DbRelease> builder)
    {
        builder.ToTable("Releases", IssueTrackerDbConfig.Schema);
        builder.HasIndex(release => release.GitlabId).IsUnique();
        builder.HasIndex(release => release.GitlabIid).IsUnique();
        builder.HasIndex(release => release.GitHubId).IsUnique();
    }
}
