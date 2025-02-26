using GreenDonut;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    [Authorize]
    public async Task<Connection<Appearance>> GetAppearances(
        PagingArguments pagingArguments,
        IAppearanceService appearanceService,
        CancellationToken cancellationToken)
    {
        Page<Appearance> page = await appearanceService.GetAllAppearancesAsync(pagingArguments, cancellationToken);
        return page.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    [Authorize]
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
