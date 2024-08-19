using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbJobConfiguration : IEntityTypeConfiguration<DbJob>
{
    public void Configure(EntityTypeBuilder<DbJob> builder)
    {
        builder.ToTable("Jobs", IssueTrackerDbConfig.Schema);
        builder.HasIndex(job => job.JobType).IsUnique();
    }
}
