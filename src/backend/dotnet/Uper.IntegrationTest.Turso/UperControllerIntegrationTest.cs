using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Uper.Domain.Request.Dto;

namespace Uper.IntegrationTest.Turso;

public class UperControllerIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.WithWebHostBuilder(builder =>
        {
            // Additional configuration for the test environment, if needed
        }).CreateClient();

    [Fact]
    public async Task CreateAsync_ShouldReturnOk()
    {
        // Arrange
        var dto = new CreateUpdateDto
        {
            Type = "TestType",
            Objects =
            [
                new() { ["Id"] = "1", ["Name"] = "Test Name" }
            ]
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        // Add a mock Authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "mock-jwt-token");

        // Act
        var response = await _client.PostAsync("/api/Uper/create", jsonContent);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Data created successfully", responseString);
    }
}
