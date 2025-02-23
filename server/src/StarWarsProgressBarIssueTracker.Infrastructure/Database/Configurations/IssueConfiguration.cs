using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Entities;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable($"{nameof(Issue)}s", IssueTrackerDbConfig.Schema);
        builder.HasIndex(issue => issue.GitlabId).IsUnique();
        builder.HasIndex(issue => issue.GitlabIid).IsUnique();
        builder.HasIndex(issue => issue.GitHubId).IsUnique();
        builder.Property(issue => issue.Title).IsRequired()
            .HasMaxLength(IssueConstants.MaxTitleLength);
        builder.Property(issue => issue.Description)
            .HasMaxLength(IssueConstants.MaxDescriptionLength);
        builder.HasOne(issue => issue.Milestone)
            .WithMany(milestone => milestone.Issues)
            .HasForeignKey($"{nameof(Milestone)}Id")
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(issue => issue.Release)
            .WithMany(release => release.Issues)
            .HasForeignKey($"{nameof(Release)}Id")
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(issue => issue.Vehicle);
        builder.HasMany(issue => issue.Labels).WithMany(label => label.Issues); // TODO delete behavior to set null
        builder.HasMany(issue => issue.LinkedIssues).WithOne(linkedIssue => linkedIssue.LinkedIssue)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(e => e.State)
            .HasConversion(
                value => new DbIssueState { Id = (int)value, Name = Enum.GetName(value) ?? ((int)value).ToString() },
                entity =>(IssueState)entity.Id);
        builder.Property(e => e.Priority)
            .HasConversion(
                value => new DbPriority { Id = (int)value, Name = Enum.GetName(value) ?? ((int)value).ToString() },
                entity =>(Priority)entity.Id);
    }
}
