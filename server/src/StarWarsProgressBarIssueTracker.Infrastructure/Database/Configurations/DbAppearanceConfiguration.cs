using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbAppearanceConfiguration : IEntityTypeConfiguration<DbAppearance>
{
    public void Configure(EntityTypeBuilder<DbAppearance> builder)
    {
        builder.ToTable("Appearances", IssueTrackerDbConfig.Schema);
        builder.HasIndex(appearance => appearance.GitlabId).IsUnique();
        builder.HasIndex(appearance => appearance.GitHubId).IsUnique();
    }
}
