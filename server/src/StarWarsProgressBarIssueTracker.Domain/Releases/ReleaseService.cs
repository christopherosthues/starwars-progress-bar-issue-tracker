using StarWarsProgressBarIssueTracker.Domain.Exceptions;

namespace StarWarsProgressBarIssueTracker.Domain.Releases;

public class ReleaseService(IDataPort<Release> dataPort) : IReleaseService
{
    public async Task<IEnumerable<Release>> GetAllReleasesAsync(CancellationToken cancellationToken = default)
    {
        return await dataPort.GetAllAsync(cancellationToken);
    }

    public async Task<Release?> GetReleaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dataPort.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Release> AddReleaseAsync(Release release, CancellationToken cancellationToken = default)
    {
        ValidateRelease(release);

        return await dataPort.AddAsync(release, cancellationToken);
    }

    private static void ValidateRelease(Release release)
    {
        var errors = new List<Exception>();
        if (string.IsNullOrWhiteSpace(release.Title))
        {
            errors.Add(new ValueNotSetException(nameof(Release.Title)));
        }

        if (release.Title.Length < ReleaseConstants.MinTitleLength)
        {
            errors.Add(new StringTooShortException(release.Title, nameof(Release.Title),
                $"The length of {nameof(Release.Title)} has to be between {ReleaseConstants.MinTitleLength} and {ReleaseConstants.MaxTitleLength}."));
        }

        if (release.Title.Length > ReleaseConstants.MaxTitleLength)
        {
            errors.Add(new StringTooLongException(release.Title, nameof(Release.Title),
                $"The length of {nameof(Release.Title)} has to be between {ReleaseConstants.MinTitleLength} and {ReleaseConstants.MaxTitleLength}."));
        }

        if (release.Notes is not null && release.Notes.Length > ReleaseConstants.MaxNotesLength)
        {
            errors.Add(new StringTooLongException(release.Notes, nameof(Release.Notes),
                $"The length of {nameof(Release.Notes)} has to be less than {ReleaseConstants.MaxNotesLength + 1}."));
        }

        if (!Enum.IsDefined(release.State) || release.State == ReleaseState.Unknown)
        {
            errors.Add(new ValueNotSetException(nameof(Release.State)));
        }

        if (errors.Count != 0)
        {
            throw new AggregateException(errors);
        }
    }

    public async Task<Release> UpdateReleaseAsync(Release release, CancellationToken cancellationToken = default)
    {
        ValidateRelease(release);

        if (!(await dataPort.ExistsAsync(release.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Release), release.Id.ToString());
        }

        return await dataPort.UpdateAsync(release, cancellationToken);
    }

    public async Task<Release> DeleteReleaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await dataPort.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Release), id.ToString());
        }

        return await dataPort.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Release> releases, CancellationToken cancellationToken = default)
    {
        var existingReleases = await dataPort.GetAllAsync(cancellationToken);

        var releasesToAdd = releases.Where(release =>
            !existingReleases.Any(existingRelease => release.GitlabId!.Equals(existingRelease.GitlabId)));

        var releasesToDelete = existingReleases.Where(existingRelease => existingRelease.GitlabId != null &&
                                                                               !releases.Any(release => release.GitlabId!.Equals(existingRelease.GitlabId)));

        await dataPort.AddRangeAsync(releasesToAdd, cancellationToken);

        await dataPort.DeleteRangeByGitlabIdAsync(releasesToDelete, cancellationToken);

        // TODO: Update milestone, resolve conflicts
    }
}
