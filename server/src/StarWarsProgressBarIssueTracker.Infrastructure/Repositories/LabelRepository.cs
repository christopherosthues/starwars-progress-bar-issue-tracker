using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class LabelRepository : IssueTrackerRepositoryBase<DbLabel>
{
    protected override IQueryable<DbLabel> GetIncludingFields()
    {
        return DbSet.Include(dbLabel => dbLabel.Issues);
    }
}
