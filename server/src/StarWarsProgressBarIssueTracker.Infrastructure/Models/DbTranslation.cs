using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbTranslationConfiguration))]
public record DbTranslation : DbEntityBase
{
    [StringLength(TranslationConstants.MaxCountryLength, MinimumLength = TranslationConstants.MinCountryLength)]
    public required string Country { get; set; }

    [MaxLength(TranslationConstants.MaxTextLength)]
    public required string Text { get; set; }
}
