using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbVehicleConfiguration : IEntityTypeConfiguration<DbVehicle>
{
    public void Configure(EntityTypeBuilder<DbVehicle> builder)
    {
        builder.ToTable("Vehicles", IssueTrackerDbConfig.Schema);
    }
}
