using HotChocolate.Types;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<ColorFormatException>]
    public async Task<Appearance> AddAppearance(string title, string color, string textColor, string? description, CancellationToken cancellationToken)
    {
        return await appearanceService.AddAppearanceAsync(new()
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
    public async Task<Appearance> UpdateAppearance(Guid id, string title, string color, string textColor, string? description, CancellationToken cancellationToken)
    {
        return await appearanceService.UpdateAppearanceAsync(new()
        {
            Id = id,
            Title = title,
            Description = description,
            Color = color,
            TextColor = textColor
        }, cancellationToken);
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Appearance> DeleteAppearance(Guid id, CancellationToken cancellationToken)
    {
        return await appearanceService.DeleteAppearanceAsync(id, cancellationToken);
    }
}
