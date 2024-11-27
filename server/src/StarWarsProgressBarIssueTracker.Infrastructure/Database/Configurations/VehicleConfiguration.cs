using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable($"{nameof(Vehicle)}s", IssueTrackerDbConfig.Schema);
        builder.HasMany(vehicle => vehicle.Appearances); // set null
        builder.HasMany(vehicle => vehicle.Translations); // cascade
        builder.HasMany(vehicle => vehicle.Photos); // cascade
    }
}
