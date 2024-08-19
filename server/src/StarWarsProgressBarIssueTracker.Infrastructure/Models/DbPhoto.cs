using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

[EntityTypeConfiguration(typeof(DbPhotoConfiguration))]
public record DbPhoto : DbEntityBase
{
    public required string FilePath { get; set; }
}
