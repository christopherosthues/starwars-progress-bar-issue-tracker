using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable($"{nameof(Photo)}s", IssueTrackerDbConfig.Schema);
        builder.Property(photo => photo.FilePath).IsRequired()
            .HasMaxLength(PhotoConstants.MaxFilePathLength);
    }
}
