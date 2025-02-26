using GreenDonut;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    [Authorize]
    public async Task<Connection<MilestoneDto>> GetMilestones(
        PagingArguments pagingArguments,
        IMilestoneService milestoneService,
        MilestoneMapper milestoneMapper,
        CancellationToken cancellationToken)
    {
        Page<Milestone> page = await milestoneService.GetAllMilestonesAsync(pagingArguments, cancellationToken);
        Page<MilestoneDto> dtoPage = new Page<MilestoneDto>(
            [..page.Items.Select(milestoneMapper.MapToMilestoneDto)], page.HasNextPage,
            page.HasPreviousPage, milestone => page.CreateCursor(milestoneMapper.MapToMilestone(milestone)), page.TotalCount);
        return dtoPage.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    [Authorize]
    public async Task<MilestoneDto?> GetMilestone(
        Guid id,
        IMilestoneService milestoneService,
        MilestoneMapper milestoneMapper,
        CancellationToken cancellationToken)
    {
        return milestoneMapper.MapToNullableMilestoneDto(await milestoneService.GetMilestoneAsync(id, cancellationToken));
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Milestone>> GetMilestoneByIdAsync(
        IReadOnlyList<Guid> ids,
        IMilestoneRepository milestoneRepository,
        CancellationToken cancellationToken = default)
    {
        return await milestoneRepository.GetMilestoneByIds(ids)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}
