namespace StarWarsProgressBarIssueTracker.App.Authorization;

public record KeycloakConfiguration
{
    public required string Audience { get; init; }

    public required string MetadataAddress { get; init; }

    public required string ValidIssuer { get; init; }

    public required string AuthorizationUrl { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string Authority { get; init; }

    public required string RegistrationUrl { get; init; }

    public required string TokenUrl { get; init; }
}
