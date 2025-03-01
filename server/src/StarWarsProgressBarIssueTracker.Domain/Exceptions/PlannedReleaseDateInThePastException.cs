namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class PlannedReleaseDateInThePastException(DateTime releaseDate)
    : IssueTrackerBaseException($"The planned release date '{releaseDate:d}' is in the past.");
