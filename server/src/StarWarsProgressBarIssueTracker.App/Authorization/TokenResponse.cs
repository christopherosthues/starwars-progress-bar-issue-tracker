using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_expires_in")]
    public required int RefreshExpiresIn { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
}
