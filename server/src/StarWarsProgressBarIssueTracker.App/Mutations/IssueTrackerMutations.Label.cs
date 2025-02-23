using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.App.Labels;
using StarWarsProgressBarIssueTracker.CodeGen.GraphQL;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<ColorFormatException>]
    [MutationFieldName(nameof(Label))]
    public partial async Task<LabelDto> AddLabel(string title, string color, string textColor, string? description,
        CancellationToken cancellationToken)
    {
        return labelMapper.MapToLabelDto(await labelService.AddLabelAsync(
            new Label { Title = title, Description = description, Color = color, TextColor = textColor },
            cancellationToken));
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<ColorFormatException>]
    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Label))]
    public partial async Task<LabelDto> UpdateLabel([ID] Guid id, string title, string color, string textColor,
        string? description, CancellationToken cancellationToken)
    {
        return labelMapper.MapToLabelDto(await labelService.UpdateLabelAsync(
            new Label
            {
                Id = id,
                Title = title,
                Description = description,
                Color = color,
                TextColor = textColor
            }, cancellationToken));
    }

    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Label))]
    public partial async Task<LabelDto> DeleteLabel([ID] Guid id, CancellationToken cancellationToken)
    {
        return labelMapper.MapToLabelDto(await labelService.DeleteLabelAsync(id, cancellationToken));
    }
}
