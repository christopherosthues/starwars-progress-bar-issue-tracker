using GreenDonut;
using GreenDonut.Data;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Issues;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Issues;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<IssueDto>> GetIssues(
        PagingArguments pagingArguments,
        IIssueService issueService,
        IssueMapper issueMapper,
        CancellationToken cancellationToken)
    {
        Page<Issue> page = await issueService.GetAllIssuesAsync(pagingArguments, cancellationToken);
        Page<IssueDto> dtoPage = new Page<IssueDto>(
            [..page.Items.Select(issueMapper.MapToIssueDto)], page.HasNextPage,
            page.HasPreviousPage, issue => page.CreateCursor(issueMapper.MapToIssue(issue)), page.TotalCount);
        return dtoPage.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<IssueDto?> GetIssue(
        Guid id,
        IIssueService issueService,
        IssueMapper issueMapper,
        CancellationToken cancellationToken)
    {
        return issueMapper.MapToNullableIssueDto(await issueService.GetIssueAsync(id, cancellationToken));
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
