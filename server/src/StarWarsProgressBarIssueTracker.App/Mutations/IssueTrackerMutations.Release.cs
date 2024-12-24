using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.CodeGen;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [MutationFieldName(nameof(Release))]
    public partial async Task<Release> AddRelease(string title, string? releaseNotes, DateTime? releaseDate, CancellationToken cancellationToken)
    {
        return await releaseService.AddReleaseAsync(new()
        {
            Title = title,
            Notes = releaseNotes,
            Date = releaseDate,
            State = ReleaseState.Planned,
        }, cancellationToken);
    }

    [MutationFieldName(nameof(Release))]
    public partial async Task<Release> UpdateRelease([ID] Guid id, string title, ReleaseState state, string? releaseNotes, DateTime? releaseDate, CancellationToken cancellationToken)
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

    [MutationFieldName(nameof(Release))]
    public partial async Task<Release> DeleteRelease([ID] Guid id, CancellationToken cancellationToken)
    {
        return await releaseService.DeleteReleaseAsync(id, cancellationToken);
    }
}
