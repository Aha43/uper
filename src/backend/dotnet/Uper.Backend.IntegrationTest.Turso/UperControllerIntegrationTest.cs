using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Uper.Backend.IntegrationTest.Turso.Tools;
using Uper.Backend.Domain.Request.Dto;
using Uper.Backend.Repository.Turso;

namespace Uper.Backend.IntegrationTest.Turso;

public class UperControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly TursoClient _tursoClient;

    private readonly HttpClient _client;

    public UperControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.ConfigureServicesForIntegrationTest();
            });
        }).CreateClient();

        var services = new ServiceCollection();
        var serviceProvider = services.ConfigureServicesForIntegrationTest().BuildServiceProvider();
        _tursoClient = serviceProvider.GetRequiredService<TursoClient>();
    }

    private string GetTableName(string test) => $"{nameof(UperControllerIntegrationTests).GetInitials()}_{test}";

    [Fact]
    public async Task CreateAsync_ShouldReturnOk()
    {
        var testHelper = new TestHelper(_tursoClient);
        await testHelper.InitializeAsync(GetTableName(nameof(CreateAsync_ShouldReturnOk)), false);

        try
        {
            // Arrange
            var dto = new CreateUpdateDto
            {
                Type = testHelper.TableName,
                Objects =
                [
                    new() { ["Id"] = "1", ["Name"] = "Test Name" }
                ]
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            // Add a mock Authorization header
            _client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", "mock-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/Uper/create", jsonContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Data created successfully", responseString);
        }
        finally
        {
            await testHelper.DisposeAsync();
        }
    }
}
