namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record LoginDto
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
}
