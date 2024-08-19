using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbPhotoConfiguration : IEntityTypeConfiguration<DbPhoto>
{
    public void Configure(EntityTypeBuilder<DbPhoto> builder)
    {
        builder.ToTable("Photos", IssueTrackerDbConfig.Schema);
    }
}
