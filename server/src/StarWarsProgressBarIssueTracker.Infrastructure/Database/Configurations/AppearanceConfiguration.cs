using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class AppearanceConfiguration : IEntityTypeConfiguration<Appearance>
{
    public void Configure(EntityTypeBuilder<Appearance> builder)
    {
        builder.ToTable($"{nameof(Appearance)}s", IssueTrackerDbConfig.Schema);
        builder.HasIndex(appearance => appearance.GitlabId).IsUnique();
        builder.HasIndex(appearance => appearance.GitHubId).IsUnique();
        builder.Property(label => label.Title).IsRequired()
            .HasMaxLength(AppearanceConstants.MaxTitleLength);
        builder.Property(label => label.Description)
            .HasMaxLength(AppearanceConstants.MaxDescriptionLength);
        builder.Property(label => label.Color).IsRequired()
            .HasMaxLength(AppearanceConstants.ColorHexLength);
        builder.Property(label => label.TextColor).IsRequired()
            .HasMaxLength(AppearanceConstants.ColorHexLength);
    }
}
