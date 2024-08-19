using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;

public class EditLabel
{
    [JsonPropertyName("new_name")]
    public string? NewName { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
