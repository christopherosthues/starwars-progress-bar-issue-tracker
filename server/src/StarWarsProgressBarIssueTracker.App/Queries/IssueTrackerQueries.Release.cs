using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<ReleaseDto>> GetReleases(
        PagingArguments pagingArguments,
        IReleaseService releaseService,
        ReleaseMapper releaseMapper,
        CancellationToken cancellationToken)
    {
        Page<Release> page = await releaseService.GetAllReleasesAsync(pagingArguments, cancellationToken);
        Page<ReleaseDto> dtoPage = new Page<ReleaseDto>(
            [..page.Items.Select(releaseMapper.MapToReleaseDto)], page.HasNextPage,
            page.HasPreviousPage, release => page.CreateCursor(releaseMapper.MapToRelease(release)), page.TotalCount);
        return dtoPage.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<ReleaseDto?> GetRelease(
        Guid id,
        IReleaseService releaseService,
        ReleaseMapper releaseMapper,
        CancellationToken cancellationToken)
    {
        return releaseMapper.MapToNullableLabelDto(await releaseService.GetReleaseAsync(id, cancellationToken));
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
