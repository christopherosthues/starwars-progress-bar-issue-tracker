using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.App.Extensions;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<Label>> GetLabels(
        PagingArguments pagingArguments,
        ILabelService labelService,
        CancellationToken cancellationToken)
    {
        Page<Label> page = await labelService.GetAllLabelsAsync(pagingArguments, cancellationToken);
        return page.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
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
