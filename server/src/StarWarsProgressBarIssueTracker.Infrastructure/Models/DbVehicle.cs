using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbVehicleConfiguration))]
public record DbVehicle : DbEntityBase
{
    [ForeignKey("AppearanceId")]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public virtual IList<DbAppearance> Appearances { get; set; } = [];

    public EngineColor EngineColor { get; set; }

    [ForeignKey("TranslationId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual IList<DbTranslation> Translations { get; set; } = [];

    [ForeignKey("PhotoId")]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual IList<DbPhoto> Photos { get; set; } = [];
}
