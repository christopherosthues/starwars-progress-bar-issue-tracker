using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    public async Task<Release> AddRelease(string title, string? releaseNotes, DateTime? releaseDate, CancellationToken cancellationToken)
    {
        return await releaseService.AddReleaseAsync(new()
        {
            Title = title,
            Notes = releaseNotes,
            Date = releaseDate,
            State = ReleaseState.Planned,
        }, cancellationToken);
    }

    public async Task<Release> UpdateRelease(Guid id, string title, ReleaseState state, string? releaseNotes, DateTime? releaseDate, CancellationToken cancellationToken)
    {
        return await releaseService.UpdateReleaseAsync(new Release
        {
            Id = id,
            Title = title,
            Notes = releaseNotes,
            Date = releaseDate,
            State = state
        }, cancellationToken);
    }

    public async Task<Release> DeleteRelease(Guid id, CancellationToken cancellationToken)
    {
        return await releaseService.DeleteReleaseAsync(id, cancellationToken);
    }
}
