using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbMilestoneConfiguration))]
public record DbMilestone : DbEntityBase
{
    [StringLength(MilestoneConstants.MaxTitleLength, MinimumLength = MilestoneConstants.MinTitleLength)]
    public required string Title { get; set; }

    [MaxLength(MilestoneConstants.MaxDescriptionLength)]
    public string? Description { get; set; }

    public MilestoneState State { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public virtual IList<DbIssue> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
