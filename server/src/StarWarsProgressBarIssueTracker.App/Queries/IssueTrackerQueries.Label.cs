using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Label>> GetLabels(CancellationToken cancellationToken)
    {
        return await labelService.GetAllLabelsAsync(cancellationToken);
    }

    public async Task<Label?> GetLabel(Guid id, CancellationToken cancellationToken)
    {
        return await labelService.GetLabelAsync(id, cancellationToken);
    }
}
