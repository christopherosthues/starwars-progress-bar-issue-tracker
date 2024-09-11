using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class AppearanceRepository(IssueTrackerContext context)
    : IssueTrackerRepositoryBase<Appearance>(context), IAppearanceRepository
{
    public override async Task<Page<Appearance>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(appearance => appearance.Title)
            .ThenBy(appearance => appearance.Id)
            .ToPageAsync(pagingArguments, true, cancellationToken);
    }

    public IQueryable<Appearance> GetAppearanceByIds(IReadOnlyList<Guid> appearanceIds)
    {
        return DbSet
            .AsNoTracking()
            .Where(appearance => appearanceIds.Contains(appearance.Id));
    }

    public override async Task<Appearance> UpdateAsync(Appearance entity, CancellationToken cancellationToken = default)
    {
        Appearance dbEntity = (await GetByIdAsync(entity.Id, cancellationToken))!;
        dbEntity.Title = entity.Title;
        dbEntity.Description = entity.Description;
        dbEntity.Color = entity.Color;
        dbEntity.TextColor = entity.TextColor;
        await Context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public IQueryable<Appearance> GetAppearancesById(IEnumerable<Guid> appearanceIds)
    {
        return DbSet.Where(dbAppearance => appearanceIds.Any(id => id.Equals(dbAppearance.Id)));
    }
}
