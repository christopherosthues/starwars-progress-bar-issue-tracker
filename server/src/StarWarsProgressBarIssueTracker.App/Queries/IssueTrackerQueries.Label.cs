using GreenDonut;
using HotChocolate.Data;
using HotChocolate.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.App.Extensions;
using StarWarsProgressBarIssueTracker.App.Labels;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseSorting]
    public async Task<Connection<GetLabelDto>> GetLabels(
        PagingArguments pagingArguments,
        ILabelService labelService,
        LabelMapper labelMapper,
        CancellationToken cancellationToken)
    {
        Page<Label> page = await labelService.GetAllLabelsAsync(pagingArguments, cancellationToken);
        Page<GetLabelDto> dtoPage = new Page<GetLabelDto>(
            [..page.Items.Select(label => labelMapper.MapToGetLabelDto(label)!)], page.HasNextPage,
            page.HasPreviousPage, label => page.CreateCursor(labelMapper.MapToLabel(label)), page.TotalCount);
        return dtoPage.ToConnectionWithTotalCount();
    }

    [Error<DomainIdNotFoundException>]
    public async Task<GetLabelDto?> GetLabel(
        ILabelService labelService,
        LabelMapper labelMapper,
        Guid id, CancellationToken cancellationToken)
    {
        return labelMapper.MapToGetLabelDto(await labelService.GetLabelAsync(id, cancellationToken));
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
