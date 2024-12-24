namespace StarWarsProgressBarIssueTracker.App.Labels;

public class GetLabelDto : LabelDto
{
    public IList<LabelIssueDto> Issues { get; init; } = [];
}
