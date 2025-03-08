using System.Net;
using System.Text;
using System.Text.Json;
using StarWarsProgressBarIssueTracker.App.Authorization;
using StarWarsProgressBarIssueTracker.App.Tests.Helpers;
using StarWarsProgressBarIssueTracker.TestHelpers;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration;

[Category(TestCategory.Integration)]
[NotInParallel(NotInParallelTests.Authentication)]
public class AuthenticationTests : IntegrationTestBase
{
    [Test]
    public async Task Login_WithCorrectCredentials_ReturnsValidAccessToken()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        LoginDto loginDto = new() { UserName = KeycloakConfig.TestUserName, Password = KeycloakConfig.TestPassword };

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? tokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(tokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(tokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(tokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(tokenResponse.TokenType).IsEqualTo("Bearer");
        }
    }

    [Test]
    public async Task Login_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(new
            {
                userName = KeycloakConfig.TestUserName,
            }), Encoding.UTF8, "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithMissingUserName_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(new
            {
                password = KeycloakConfig.TestPassword,
            }), Encoding.UTF8, "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithWrongCredentials_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        LoginDto loginDto = new() { UserName = KeycloakConfig.TestUserName, Password = "wrong password" };
        // Act
        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Refresh_WithCorrectRefreshToken_ReturnsValidAccessToken()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        LoginDto loginDto = new()
        {
            UserName = KeycloakConfig.TestUserName,
            Password = KeycloakConfig.TestPassword
        };

        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? tokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(tokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(tokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(tokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(tokenResponse.TokenType).IsEqualTo("Bearer");
        }

        // Act
        response = await httpClient.PostAsync("refresh",
            new StringContent(JsonSerializer.Serialize(new RefreshTokenDto
                {
                    RefreshToken = tokenResponse.RefreshToken!
                }), Encoding.UTF8,
                "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? refreshTokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(refreshTokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(refreshTokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty().And
                .IsNotEqualTo(tokenResponse.AccessToken);
            await Assert.That(refreshTokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty().And
                .IsNotEqualTo(tokenResponse.RefreshToken);
            await Assert.That(refreshTokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(refreshTokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(refreshTokenResponse.TokenType).IsEqualTo("Bearer");
        }
    }

    [Test]
    public async Task Refresh_WithWrongRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        LoginDto loginDto = new()
        {
            UserName = KeycloakConfig.TestUserName,
            Password = KeycloakConfig.TestPassword
        };

        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? tokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(tokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(tokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(tokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(tokenResponse.TokenType).IsEqualTo("Bearer");
        }

        // Act
        response = await httpClient.PostAsync("refresh",
            new StringContent(JsonSerializer.Serialize(new RefreshTokenDto
                {
                    RefreshToken = "wrong refresh token"
                }), Encoding.UTF8,
                "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Refresh_WithMissingRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        LoginDto loginDto = new()
        {
            UserName = KeycloakConfig.TestUserName,
            Password = KeycloakConfig.TestPassword
        };

        HttpResponseMessage response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? tokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(tokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(tokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(tokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(tokenResponse.TokenType).IsEqualTo("Bearer");
        }

        // Act
        response = await httpClient.PostAsync("refresh",
            new StringContent(JsonSerializer.Serialize(new { }), Encoding.UTF8,
                "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_ReturnsCreated()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser";
        const string email = "NewTestUser@test.com";
        const string password = "NewTestPassword";
        RegistrationDto registrationDto = new()
        {
            Username = username,
            Email = email,
            Password = password,
            FirstName = "AnotherTest",
            LastName = "AnotherUser"
        };

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(registrationDto), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
    }

    [Test]
    public async Task Register_WithMissingUsername_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string email = "NewTestUser@test.com";
        const string password = "NewTestPassword";

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(new
            {
                Email = email,
                Password = password,
                FirstName = "AnotherTest",
                LastName = "AnotherUser"
            }), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithMissingEmail_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser";
        const string password = "NewTestPassword";

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(new
            {
                Username = username,
                Password = password,
                FirstName = "AnotherTest",
                LastName = "AnotherUser"
            }), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser";
        const string email = "NewTestUser@test.com";

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(new
            {
                Username = username,
                Email = email,
                FirstName = "AnotherTest",
                LastName = "AnotherUser"
            }), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithMissingFirstName_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser";
        const string email = "NewTestUser@test.com";
        const string password = "NewTestPassword";

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(new
            {
                Username = username,
                Email = email,
                Password = password,
                LastName = "AnotherUser"
            }), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Register_WithMissingLastName_ReturnsBadRequest()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser";
        const string email = "NewTestUser@test.com";
        const string password = "NewTestPassword";

        // Act
        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(new
            {
                Username = username,
                Email = email,
                Password = password,
                FirstName = "AnotherTest"
            }), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Login_WithNewlyCreatedUser_ReturnsAccessToken()
    {
        // Arrange
        HttpClient httpClient = CreateHttpClient();
        const string username = "NewTestUser2";
        const string email = "NewTestUser2@test.com";
        const string password = "NewTestPassword2";
        RegistrationDto registrationDto = new()
        {
            Username = username,
            Email = email,
            Password = password,
            FirstName = "Test2",
            LastName = "User2"
        };

        HttpResponseMessage response = await httpClient.PostAsync("register",
            new StringContent(JsonSerializer.Serialize(registrationDto), Encoding.UTF8, "application/json"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

        LoginDto loginDto = new()
        {
            UserName = username,
            Password = password
        };

        // Act
        response = await httpClient.PostAsync("login",
            new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        TokenResponse? tokenResponse =
            await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync());
        await Assert.That(tokenResponse).IsNotNull();
        using (Assert.Multiple())
        {
            await Assert.That(tokenResponse!.AccessToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.RefreshToken).IsNotNull().And.IsNotEmpty();
            await Assert.That(tokenResponse.ExpiresIn).IsEqualTo(300);
            await Assert.That(tokenResponse.RefreshExpiresIn).IsEqualTo(1800);
            await Assert.That(tokenResponse.TokenType).IsEqualTo("Bearer");
        }
    }
}
