using AuthAPI.Controllers;
using System.Net.Http.Json;

namespace Shared.Tests.AuthAPITests.Controllers;

[Collection(TestConstants.Auth)]
public class AuthControllerTests(AuthTestContextFixture context)
{
    private readonly HttpClient _client = context.Client;

    // ---------------------------------------------
    // 1. REGISTER USER
    // ---------------------------------------------
    [Fact]
    public async Task Register_ShouldReturn200_AndCreateUser()
    {
        // Arrange
        var request = new AuthController.AuthRegisterRequest(
            Email: "newuser@test.com",
            Password: "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body!["message"].Should().Be("User registered successfully");
    }

    // ---------------------------------------------
    // 2. LOGIN AFTER REGISTRATION → Should return JWT token
    // ---------------------------------------------
    [Fact]
    public async Task Login_ShouldReturnJwtToken_WhenCredentialsAreValid()
    {
        // Arrange: first register a new user
        var registerReq = new AuthController.AuthRegisterRequest(
            Email: "loginuser@test.com",
            Password: "Password123!"
        );

        await _client.PostAsJsonAsync("/api/auth/register", registerReq);

        var loginReq = new AuthController.AuthLoginRequest(
            Email: "loginuser@test.com",
            Password: "Password123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginReq);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json!.Should().ContainKey("token");
        json["token"].Should().NotBeNullOrWhiteSpace();
    }

    // ---------------------------------------------
    // 3. LOGIN WITH WRONG PASSWORD
    // ---------------------------------------------
    [Fact]
    public async Task Login_ShouldReturn401_WhenPasswordIsInvalid()
    {
        // Arrange
        var registerReq = new AuthController.AuthRegisterRequest(
            Email: "wrongpass@test.com",
            Password: "Password123!"
        );

        await _client.PostAsJsonAsync("/api/auth/register", registerReq);

        var loginReq = new AuthController.AuthLoginRequest(
            Email: "wrongpass@test.com",
            Password: "WrongPassword"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginReq);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ---------------------------------------------
    // 4. LOGOUT REQUIRES JWT → Should return 200
    // ---------------------------------------------
    [Fact]
    public async Task Logout_ShouldReturn200_WhenAuthenticated()
    {
        // Arrange: register & login to get token
        var email = "logoutuser@test.com";
        var password = "Password123!";

        await _client.PostAsJsonAsync("/api/auth/register",
            new AuthController.AuthRegisterRequest(email, password));

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new AuthController.AuthLoginRequest(email, password)
        );

        var json = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = json!["token"];

        // Add JWT to headers
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await logoutResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body!["message"].Should().Be("Logged out successfully");
    }
}

