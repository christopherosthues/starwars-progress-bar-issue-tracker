using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    // [UseFiltering]
    public async Task<Connection<Label>> GetLabels(
        PagingArguments pagingArguments,
        ILabelService labelService,
        CancellationToken cancellationToken)
    {
        Page<Label> page = await labelService.GetAllLabelsAsync(pagingArguments, cancellationToken);
        Connection<Label> connection = new Connection<Label>(
            page.Items.Select(t => new Edge<Label>(t, page.CreateCursor)).ToArray(),
            new ConnectionPageInfo(
                page.HasNextPage,
                page.HasPreviousPage,
                CreateCursor(page.First, page.CreateCursor),
                CreateCursor(page.Last, page.CreateCursor)),
            page.TotalCount ?? 0);
        return connection;
    }

    private static string? CreateCursor<T>(T? item, Func<T, string> createCursor) where T : class
        => item is null ? null : createCursor(item);

    public async Task<Label?> GetLabel(
        ILabelService labelService,
        Guid id, CancellationToken cancellationToken)
    {
        return await labelService.GetLabelAsync(id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, Label>> GetLabelByIdAsync(
        IReadOnlyList<Guid> ids,
        ILabelRepository labelRepository,
        CancellationToken cancellationToken = default)
    {
        return await labelRepository.GetLabelByIds(ids)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}
