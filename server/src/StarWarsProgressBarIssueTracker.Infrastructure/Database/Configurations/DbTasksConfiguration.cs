using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbTasksConfiguration : IEntityTypeConfiguration<DbTask>
{
    public void Configure(EntityTypeBuilder<DbTask> builder)
    {
        builder.ToTable("Tasks", IssueTrackerDbConfig.Schema);
    }
}
