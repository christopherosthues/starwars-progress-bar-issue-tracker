using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;

public class EditMilestone
{
    public const string ActivateEvent = "activate";
    public const string CloseEvent = "close";

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("state_event")]
    public string? StateEvent { get; set; }
}
