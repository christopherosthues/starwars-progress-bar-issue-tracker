using GreenDonut;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Appearance>> GetAppearances(
        PagingArguments pagingArguments,
        IAppearanceService appearanceService,
        CancellationToken cancellationToken)
    {
        return await appearanceService.GetAllAppearancesAsync(pagingArguments, cancellationToken);
    }

    public async Task<Appearance?> GetAppearance(
        Guid id,
        IAppearanceService appearanceService,
        CancellationToken cancellationToken)
    {
        return await appearanceService.GetAppearanceAsync(id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Appearance>> GetAppearanceByIdAsync(
        IReadOnlyList<Guid> ids,
        IAppearanceRepository appearanceRepository,
        CancellationToken cancellationToken = default)
    {
        return await appearanceRepository.GetAppearanceByIds(ids)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}
