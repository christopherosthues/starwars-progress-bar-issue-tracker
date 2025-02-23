using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Entities;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbTaskConfiguration : IEntityTypeConfiguration<DbTask>
{
    public void Configure(EntityTypeBuilder<DbTask> builder)
    {
        builder.ToTable("Tasks", IssueTrackerDbConfig.Schema);
    }
}
