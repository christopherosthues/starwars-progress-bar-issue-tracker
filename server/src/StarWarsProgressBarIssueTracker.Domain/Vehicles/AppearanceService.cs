using System.Text.RegularExpressions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public partial class AppearanceService(IDataPort<Appearance> dataPort) : IAppearanceService
{
    public async Task<IEnumerable<Appearance>> GetAllAppearancesAsync(CancellationToken cancellationToken = default)
    {
        return await dataPort.GetAllAsync(cancellationToken);
    }

    public async Task<Appearance?> GetAppearanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dataPort.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Appearance> AddAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default)
    {
        ValidateAppearance(appearance);

        return await dataPort.AddAsync(appearance, cancellationToken);
    }

    private static void ValidateAppearance(Appearance appearance)
    {
        var errors = new List<Exception>();
        if (string.IsNullOrWhiteSpace(appearance.Title))
        {
            errors.Add(new ValueNotSetException(nameof(Appearance.Title)));
        }

        if (appearance.Title.Length < AppearanceConstants.MinTitleLength)
        {
            errors.Add(new StringTooShortException(appearance.Title, nameof(Appearance.Title),
                $"The length of {nameof(Appearance.Title)} has to be between {AppearanceConstants.MinTitleLength} and {AppearanceConstants.MaxTitleLength}."));
        }

        if (appearance.Title.Length > AppearanceConstants.MaxTitleLength)
        {
            errors.Add(new StringTooLongException(appearance.Title, nameof(Appearance.Title),
                $"The length of {nameof(Appearance.Title)} has to be between {AppearanceConstants.MinTitleLength} and {AppearanceConstants.MaxTitleLength}."));
        }

        if (appearance.Description is not null && appearance.Description.Length > AppearanceConstants.MaxDescriptionLength)
        {
            errors.Add(new StringTooLongException(appearance.Description, nameof(Appearance.Description),
                $"The length of {nameof(Appearance.Description)} has to be less than {AppearanceConstants.MaxDescriptionLength + 1}."));
        }

        if (string.IsNullOrWhiteSpace(appearance.Color))
        {
            errors.Add(new ValueNotSetException(nameof(Appearance.Color)));
        }

        var regexMatcher = ColorHexCodeRegex();
        if (!regexMatcher.Match(appearance.Color).Success)
        {
            errors.Add(new ColorFormatException(appearance.Color, nameof(Appearance.Color)));
        }

        if (string.IsNullOrWhiteSpace(appearance.TextColor))
        {
            errors.Add(new ValueNotSetException(nameof(Appearance.TextColor)));
        }

        if (!regexMatcher.Match(appearance.TextColor).Success)
        {
            errors.Add(new ColorFormatException(appearance.TextColor, nameof(Appearance.TextColor)));
        }

        if (errors.Count != 0)
        {
            throw new AggregateException(errors);
        }
    }

    public async Task<Appearance> UpdateAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default)
    {
        ValidateAppearance(appearance);

        if (!(await dataPort.ExistsAsync(appearance.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Appearance), appearance.Id.ToString());
        }

        return await dataPort.UpdateAsync(appearance, cancellationToken);
    }

    public async Task<Appearance> DeleteAppearanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await dataPort.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Appearance), id.ToString());
        }

        return await dataPort.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Appearance> appearances, CancellationToken cancellationToken = default)
    {
        var existingAppearances = await dataPort.GetAllAsync(cancellationToken);

        var appearancesToAdd = appearances.Where(appearance =>
            !existingAppearances.Any(existingAppearance => appearance.GitlabId!.Equals(existingAppearance.GitlabId)));

        var appearancesToDelete = existingAppearances.Where(existingAppearance => existingAppearance.GitlabId != null &&
            !appearances.Any(appearance => appearance.GitlabId!.Equals(existingAppearance.GitlabId)));

        await dataPort.AddRangeAsync(appearancesToAdd, cancellationToken);

        await dataPort.DeleteRangeByGitlabIdAsync(appearancesToDelete, cancellationToken);

        // TODO: Update appearances, resolve conflicts
    }

    [GeneratedRegex("^#[a-fA-F0-9]{6}$")]
    private static partial Regex ColorHexCodeRegex();
}
