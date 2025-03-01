namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class ReleaseDateNotSetException() : IssueTrackerBaseException("The release is planned but the release date is not set.");
