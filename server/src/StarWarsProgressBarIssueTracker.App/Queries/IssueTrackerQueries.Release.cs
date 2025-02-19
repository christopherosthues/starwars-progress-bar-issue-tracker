using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<Release>> GetReleases(
        PagingArguments pagingArguments,
        IReleaseService releaseService,
        CancellationToken cancellationToken)
    {
        Page<Release> page = await releaseService.GetAllReleasesAsync(pagingArguments, cancellationToken);
        return page.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Release?> GetRelease(
        Guid id,
        IReleaseService releaseService,
        CancellationToken cancellationToken)
    {
        return await releaseService.GetReleaseAsync(id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Release>> GetReleaseByIdAsync(
        IReadOnlyList<Guid> ids,
        IReleaseRepository releaseRepository,
        CancellationToken cancellationToken = default)
    {
        return await releaseRepository.GetReleaseByIds(ids)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}
