using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbAppearanceConfiguration))]
public record DbAppearance : DbEntityBase
{
    [StringLength(AppearanceConstants.MaxTitleLength, MinimumLength = AppearanceConstants.MinTitleLength)]
    public required string Title { get; set; }

    [MaxLength(AppearanceConstants.MaxDescriptionLength)]
    public string? Description { get; set; }

    [StringLength(AppearanceConstants.ColorHexLength, MinimumLength = AppearanceConstants.ColorHexLength)]
    public required string Color { get; set; }

    [StringLength(AppearanceConstants.ColorHexLength, MinimumLength = AppearanceConstants.ColorHexLength)]
    public required string TextColor { get; set; }

    public string? GitlabId { get; set; }

    public string? GitHubId { get; set; }
}
