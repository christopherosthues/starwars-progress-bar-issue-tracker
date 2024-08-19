namespace StarWarsProgressBarIssueTracker.Domain.Issues;

public enum LinkType
{
    Unknown = 0,
    Blocks = 1,
    IsBlockedBy = 2,
    RelatesTo = 3,
    IsRelatedTo = 4,
    Duplicates = 5,
    IsDuplicatedBy = 6,
}
