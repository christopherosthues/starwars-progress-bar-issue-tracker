using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Issues;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<Issue>> GetIssues(
        PagingArguments pagingArguments,
        IIssueService issueService,
        CancellationToken cancellationToken)
    {
        Page<Issue> page = await issueService.GetAllIssuesAsync(pagingArguments, cancellationToken);
        return page.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Issue?> GetIssue(
        Guid id,
        IIssueService issueService,
        CancellationToken cancellationToken)
    {
        return await issueService.GetIssueAsync(id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Issue>> GetIssueByIdAsync(
        IReadOnlyList<Guid> ids,
        IIssueRepository issuesRepository,
        CancellationToken cancellationToken = default)
    {
        return await issuesRepository.GetIssueByIds(ids)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}
