using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbTranslationConfiguration : IEntityTypeConfiguration<DbTranslation>
{
    public void Configure(EntityTypeBuilder<DbTranslation> builder)
    {
        builder.ToTable("Translations", IssueTrackerDbConfig.Schema);
    }
}
