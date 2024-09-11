using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<Milestone>> GetMilestones(
        PagingArguments pagingArguments,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        Page<Milestone> page = await milestoneService.GetAllMilestonesAsync(pagingArguments, cancellationToken);
        return page.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Milestone?> GetMilestone(
        Guid id,
        IMilestoneService milestoneService,
        CancellationToken cancellationToken)
    {
        return await milestoneService.GetMilestoneAsync(id, cancellationToken);
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
