using HotChocolate.Types;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<ColorFormatException>]
    public async Task<Label> AddLabel(string title, string color, string textColor, string? description, CancellationToken cancellationToken)
    {
        return await labelService.AddLabelAsync(new()
        {
            Title = title,
            Description = description,
            Color = color,
            TextColor = textColor
        }, cancellationToken);
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<ColorFormatException>]
    [Error<DomainIdNotFoundException>]
    public async Task<Label> UpdateLabel(Guid id, string title, string color, string textColor, string? description, CancellationToken cancellationToken)
    {
        return await labelService.UpdateLabelAsync(new()
        {
            Id = id,
            Title = title,
            Description = description,
            Color = color,
            TextColor = textColor
        }, cancellationToken);
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Label> DeleteLabel(Guid id, CancellationToken cancellationToken)
    {
        return await labelService.DeleteLabelAsync(id, cancellationToken);
    }
}
