using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Configurations;

public class DbIssueLinkConfiguration : IEntityTypeConfiguration<DbIssueLink>
{
    public void Configure(EntityTypeBuilder<DbIssueLink> builder)
    {
        builder.ToTable("IssueLinks", IssueTrackerDbConfig.Schema);
    }
}
