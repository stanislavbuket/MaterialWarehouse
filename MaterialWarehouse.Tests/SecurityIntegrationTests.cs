using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace MaterialWarehouse.Tests;

public class SecurityIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SecurityIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private string GenerateTestJwtToken(string role, int userId = 1)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("SuperSecretKey123456789012345678901234567890");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "integration_test_user"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "MaterialWarehouseAPI",
            Audience = "MaterialWarehouseClients",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private HttpClient CreateClientWithToken(string role, int userId = 1)
    {
        var client = _factory.CreateClient();
        var token = GenerateTestJwtToken(role, userId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task GetMaterials_ReturnsSuccess_WithoutAuthentication()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/materials");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdjustStock_ReturnsUnauthorized_WhenAnonymous()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/materials/1/adjust?amount=10", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdjustStock_ReturnsForbidden_WhenRegisteredUser()
    {
        // Arrange
        var client = CreateClientWithToken("Registered");

        // Act
        var response = await client.PostAsync("/api/materials/1/adjust?amount=10", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdjustStock_ReturnsOkOrBadRequest_WhenAdminOrManager()
    {
        // Arrange
        var client = CreateClientWithToken("Manager");

        // Act
        var response = await client.PostAsync("/api/materials/1/adjust?amount=10", null);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_ReturnsUnauthorized_WhenAnonymous()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { UserId = 1, Items = new[] { new { MaterialId = 1, Quantity = 5 } } };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrBadRequest_WhenAuthenticated()
    {
        // Arrange
        var client = CreateClientWithToken("Registered");
        var request = new { UserId = 1, Items = new[] { new { MaterialId = 1, Quantity = 5 } } };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetLowStockReport_ReturnsUnauthorized_WhenAnonymous()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/reports/low-stock");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLowStockReport_ReturnsForbidden_WhenRegisteredUser()
    {
        // Arrange
        var client = CreateClientWithToken("Registered");

        // Act
        var response = await client.GetAsync("/api/reports/low-stock");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetLowStockReport_ReturnsOk_WhenManager()
    {
        // Arrange
        var client = CreateClientWithToken("Manager");

        // Act
        var response = await client.GetAsync("/api/reports/low-stock");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDeficitReport_ReturnsOk_WhenAdmin()
    {
        // Arrange
        var client = CreateClientWithToken("Admin");

        // Act
        var response = await client.GetAsync("/api/reports/deficit");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
