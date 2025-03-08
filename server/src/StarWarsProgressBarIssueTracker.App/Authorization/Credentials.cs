using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record Credentials
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("value")]
    public required string Value { get; init; }

    [JsonPropertyName("temporary")]
    public required bool Temporary { get; init; }
}
