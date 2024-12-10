namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class DomainIdNotFoundException(string domain, string id) : IssueTrackerBaseException($"No {domain} found with id '{id}'.");
