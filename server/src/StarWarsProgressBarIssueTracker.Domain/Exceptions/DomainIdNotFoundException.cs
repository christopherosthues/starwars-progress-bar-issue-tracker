namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class DomainIdNotFoundException(string domain, string id) : IssueTrackerExceptionBase($"No {domain} found with id '{id}'.");
