namespace StarWarsProgressBarIssueTracker.Domain.Exceptions;

public class DomainIdNotFoundException(string domain, string id) : Exception($"No {domain} found with id '{id}'.");
