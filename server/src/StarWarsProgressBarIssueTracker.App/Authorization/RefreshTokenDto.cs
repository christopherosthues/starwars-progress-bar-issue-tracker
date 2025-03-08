namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}
