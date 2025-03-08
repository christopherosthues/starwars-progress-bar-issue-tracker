using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record NewUser
{
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("firstName")]
    public required string FirstName { get; init; }

    [JsonPropertyName("lastName")]
    public required string LastName { get; init; }

    [JsonPropertyName("emailVerified")]
    public required bool EmailVerified { get; init; }

    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    [JsonPropertyName("credentials")]
    public required List<Credentials> Credentials { get; init; }
}
