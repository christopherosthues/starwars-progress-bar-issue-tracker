using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels", IssueTrackerDbConfig.Schema);
        builder.HasIndex(label => label.GitlabId).IsUnique();
        builder.HasIndex(label => label.GitHubId).IsUnique();
        builder.Property(label => label.Title).IsRequired()
            .HasMaxLength(LabelConstants.MaxTitleLength);
        builder.Property(label => label.Description)
            .HasMaxLength(LabelConstants.MaxDescriptionLength);
        builder.Property(label => label.Color)
            .HasMaxLength(LabelConstants.ColorHexLength);
        builder.Property(label => label.TextColor)
            .HasMaxLength(LabelConstants.ColorHexLength);
        // builder.HasMany(label => label.Issues).WithMany(issue => issue.Labels);
    }
}
