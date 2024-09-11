using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable($"{nameof(Translation)}s", IssueTrackerDbConfig.Schema);
        builder.Property(translation => translation.Country).IsRequired()
            .HasMaxLength(TranslationConstants.MaxCountryLength);
        builder.Property(translation => translation.Text).IsRequired()
            .HasMaxLength(TranslationConstants.MaxTextLength);
    }
}
