using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbIssueConfiguration))]
public record DbIssue : DbEntityBase
{
    [StringLength(IssueConstants.MaxTitleLength, MinimumLength = IssueConstants.MinTitleLength)]
    public required string Title { get; set; }

    [MaxLength(IssueConstants.MaxDescriptionLength)]
    public string? Description { get; set; }

    public Priority Priority { get; set; }

    public IssueState State { get; set; }

    [ForeignKey("MilestoneId")]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public DbMilestone? Milestone { get; set; }

    [ForeignKey("ReleaseId")]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public DbRelease? Release { get; set; }

    [ForeignKey("VehicleId")]
    public DbVehicle? Vehicle { get; set; }

    [ForeignKey("LabelId")]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public virtual IList<DbLabel> Labels { get; set; } = [];

    [ForeignKey("LinkedIssueId")]
    public virtual IList<DbIssueLink> LinkedIssues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
