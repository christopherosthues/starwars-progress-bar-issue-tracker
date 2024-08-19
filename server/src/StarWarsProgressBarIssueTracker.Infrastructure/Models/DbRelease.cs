using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbReleaseConfiguration))]
public record DbRelease : DbEntityBase
{
    [StringLength(ReleaseConstants.MaxTitleLength, MinimumLength = ReleaseConstants.MinTitleLength)]
    public required string Title { get; set; }

    [MaxLength(ReleaseConstants.MaxNotesLength)]
    public string? Notes { get; set; }

    public ReleaseState State { get; set; }

    public DateTime? Date { get; set; }

    [Column("IssueId")]
    public virtual IList<DbIssue> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
