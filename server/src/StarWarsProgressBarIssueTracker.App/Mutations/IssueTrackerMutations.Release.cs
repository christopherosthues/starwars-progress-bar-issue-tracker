using HotChocolate.Authorization;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.App.Releases;
using StarWarsProgressBarIssueTracker.CodeGen.GraphQL;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Release))]
    [Authorize]
    public partial async Task<ReleaseDto> AddRelease(string title, string? releaseNotes, DateTime? releaseDate,
        CancellationToken cancellationToken)
    {
        return releaseMapper.MapToReleaseDto(await releaseService.AddReleaseAsync(new()
        {
            Title = title,
            Notes = releaseNotes,
            Date = releaseDate,
            State = ReleaseState.Open,
        }, cancellationToken));
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Release))]
    [Authorize]
    public partial async Task<ReleaseDto> UpdateRelease([ID] Guid id, string title, ReleaseState state,
        string? releaseNotes, DateTime? releaseDate, CancellationToken cancellationToken)
    {
        return releaseMapper.MapToReleaseDto(await releaseService.UpdateReleaseAsync(new Release
        {
            Id = id,
            Title = title,
            Notes = releaseNotes,
            Date = releaseDate,
            State = state
        }, cancellationToken));
    }

    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Release))]
    [Authorize]
    public partial async Task<ReleaseDto> DeleteRelease([ID] Guid id, CancellationToken cancellationToken)
    {
        return releaseMapper.MapToReleaseDto(await releaseService.DeleteReleaseAsync(id, cancellationToken));
    }
}
