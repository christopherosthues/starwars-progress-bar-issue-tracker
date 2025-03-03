using GreenDonut.Data;

namespace StarWarsProgressBarIssueTracker.Domain.Labels;

public interface ILabelService
{
    Task<Page<Label>> GetAllLabelsAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default);

    Task<Label?> GetLabelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Label> AddLabelAsync(Label label, CancellationToken cancellationToken = default);

    Task<Label> UpdateLabelAsync(Label label, CancellationToken cancellationToken = default);

    Task<Label> DeleteLabelAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Label> labels, CancellationToken cancellationToken = default);
}
