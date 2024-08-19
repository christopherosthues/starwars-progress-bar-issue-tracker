using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbLabelConfiguration : IEntityTypeConfiguration<DbLabel>
{
    public void Configure(EntityTypeBuilder<DbLabel> builder)
    {
        builder.ToTable("Labels", IssueTrackerDbConfig.Schema);
        builder.HasIndex(label => label.GitlabId).IsUnique();
        builder.HasIndex(label => label.GitHubId).IsUnique();
    }
}
