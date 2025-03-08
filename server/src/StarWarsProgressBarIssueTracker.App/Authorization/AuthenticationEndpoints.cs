using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace StarWarsProgressBarIssueTracker.App.Authorization;

public static class AuthenticationEndpoints
{
    internal static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("login",
            async (HttpClient httpClient, IOptions<KeycloakConfiguration> keycloakConfiguration,
                [FromBody] LoginDto user) => await LoginAsync(keycloakConfiguration, user, httpClient));

        app.MapPost("refresh",
            async (HttpClient httpClient, IOptions<KeycloakConfiguration> keycloakConfiguration,
                    [FromBody] RefreshTokenDto refreshToken) =>
                await RefreshTokenAsync(keycloakConfiguration, refreshToken, httpClient));

        app.MapPost("register",
            async (HttpClient httpClient, IOptions<KeycloakConfiguration> keycloakConfiguration,
                [FromBody] RegistrationDto user) => await RegisterAsync(keycloakConfiguration, httpClient, user));
    }

    private static async Task<IResult> LoginAsync(IOptions<KeycloakConfiguration> keycloakConfiguration, LoginDto user, HttpClient httpClient)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", keycloakConfiguration.Value.ClientId },
            { "client_secret", keycloakConfiguration.Value.ClientSecret },
            { "username", user.UserName },
            { "password", user.Password },
        };
        HttpResponseMessage response = await httpClient.PostAsync(keycloakConfiguration.Value.TokenUrl,
            new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Invalid username or password", statusCode: (int)HttpStatusCode.Unauthorized);
        }

        TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (tokenResponse is null)
        {
            return Results.InternalServerError();
        }

        return Results.Ok(tokenResponse);
    }

    private static async Task<IResult> RefreshTokenAsync(IOptions<KeycloakConfiguration> keycloakConfiguration, RefreshTokenDto refreshToken,
        HttpClient httpClient)
    {
        Dictionary<string, string> parameters = new()
        {
            { "client_id", keycloakConfiguration.Value.ClientId },
            { "client_secret", keycloakConfiguration.Value.ClientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken.RefreshToken }
        };

        HttpResponseMessage response = await httpClient.PostAsync(keycloakConfiguration.Value.TokenUrl,
            new FormUrlEncodedContent(parameters));
        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to refresh token", statusCode: (int)HttpStatusCode.Unauthorized);
        }

        TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        if (tokenResponse is null)
        {
            return Results.InternalServerError();
        }

        return Results.Ok(tokenResponse);
    }

    private static async Task<IResult> RegisterAsync(IOptions<KeycloakConfiguration> keycloakConfiguration, HttpClient httpClient, RegistrationDto user)
    {
        Dictionary<string, string> tokenParameters = new()
        {
            { "grant_type", "client_credentials" },
            { "client_id", keycloakConfiguration.Value.ClientId },
            { "client_secret", keycloakConfiguration.Value.ClientSecret }
        };
        HttpResponseMessage tokenResponse = await httpClient.PostAsync(keycloakConfiguration.Value.TokenUrl,
            new FormUrlEncodedContent(tokenParameters));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            Results.InternalServerError();
        }

        TokenResponse? responseContent = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
        if (responseContent is null)
        {
            return Results.InternalServerError();
        }

        NewUser newUser = new()
        {
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailVerified = true,
            Enabled = true,
            Credentials =
            [
                new Credentials { Temporary = false, Type = "password", Value = user.Password }
            ]
        };
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, keycloakConfiguration.Value.RegistrationUrl)
        {
            Headers =
            {
                { "Authorization", $"Bearer {responseContent.AccessToken}" },
            },
            Content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json"),
            Method = HttpMethod.Post,
        };
        HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Could not register user");
        }

        return Results.Created();
    }
}
