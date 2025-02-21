using System.Text.RegularExpressions;
using GreenDonut.Data;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;

namespace StarWarsProgressBarIssueTracker.App.Appearances;

public partial class AppearanceService(
    IAppearanceRepository appearanceRepository,
    IAppearanceByIdDataLoader appearanceByIdDataLoader) : IAppearanceService
{
    public async Task<Page<Appearance>> GetAllAppearancesAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await appearanceRepository.GetAllAsync(pagingArguments, cancellationToken);
    }

    public async Task<Appearance?> GetAppearanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await appearanceByIdDataLoader.LoadAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            throw new DomainIdNotFoundException(nameof(Appearance), id.ToString());
        }
    }

    public async Task<Appearance> AddAppearanceAsync(Appearance appearance,
        CancellationToken cancellationToken = default)
    {
        ValidateAppearance(appearance);

        return await appearanceRepository.AddAsync(appearance, cancellationToken);
    }

    private static void ValidateAppearance(Appearance appearance)
    {
        List<Exception> errors = [];
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

        if (appearance.Description is not null &&
            appearance.Description.Length > AppearanceConstants.MaxDescriptionLength)
        {
            errors.Add(new StringTooLongException(appearance.Description, nameof(Appearance.Description),
                $"The length of {nameof(Appearance.Description)} has to be less than {AppearanceConstants.MaxDescriptionLength + 1}."));
        }

        if (string.IsNullOrWhiteSpace(appearance.Color))
        {
            errors.Add(new ValueNotSetException(nameof(Appearance.Color)));
        }

        Regex regexMatcher = ColorHexCodeRegex();
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

    public async Task<Appearance> UpdateAppearanceAsync(Appearance appearance,
        CancellationToken cancellationToken = default)
    {
        ValidateAppearance(appearance);

        if (!(await appearanceRepository.ExistsAsync(appearance.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Appearance), appearance.Id.ToString());
        }

        return await appearanceRepository.UpdateAsync(appearance, cancellationToken);
    }

    public async Task<Appearance> DeleteAppearanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await appearanceRepository.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Appearance), id.ToString());
        }

        return await appearanceRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Appearance> appearances,
        CancellationToken cancellationToken = default)
    {
        // var existingAppearances = await appearanceRepository.GetAllAsync(cancellationToken);
        //
        // var appearancesToAdd = appearances.Where(appearance =>
        //     !existingAppearances.Any(existingAppearance => appearance.GitlabId!.Equals(existingAppearance.GitlabId)));
        //
        // var appearancesToDelete = existingAppearances.Where(existingAppearance => existingAppearance.GitlabId != null &&
        //     !appearances.Any(appearance => appearance.GitlabId!.Equals(existingAppearance.GitlabId)));
        //
        // await appearanceRepository.AddRangeAsync(appearancesToAdd, cancellationToken);
        //
        // await appearanceRepository.DeleteRangeByGitlabIdAsync(appearancesToDelete, cancellationToken);

        await Task.CompletedTask;

        // TODO: Update appearances, resolve conflicts
    }

    [GeneratedRegex("^#[a-fA-F0-9]{6}$")]
    private static partial Regex ColorHexCodeRegex();
}
