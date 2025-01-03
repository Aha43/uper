using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Uper.Client.Domain;
using Uper.Client.Domain.Abstraction;

namespace Uper.Client.Http;

public class UperClient(HttpClient httpClient, ILogger<UperClient> logger) : IUperClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task CreateAsync<T>(T entity) where T : IEntity
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new ArgumentException("Entity Id cannot be empty.");
        }
        var typeName = typeof(T).Name;

        logger.LogInformation("Creating {Type} entity with id {Id}", typeName, entity.Id);

        var json = GetCreateUpdateJson(entity);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/uper/create", content);
        await EnsureSuccessAsync(response, $"creating {typeName} entity with id {entity.Id}");

        logger.LogInformation("{Type} entity with id {Id} created", typeName, entity.Id);
    }

    public async Task DeleteAsync<T>(string id) where T : IEntity
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException($"{nameof(id)} cannot be empty.");
        }
        var typeName = typeof(T).Name;

        logger.LogInformation("Deleting {Type} entity with id {Id}", typeName, id);

        var response = await httpClient.DeleteAsync($"api/uper/{typeName}/{id}");
        await EnsureSuccessAsync(response, $"deleting {typeName} with id {id}");

        logger.LogInformation("{Type} entity with id {Id} deleted", typeName, id);
    }

    public async Task<IEnumerable<T>?> GetAllAsync<T>() where T : IEntity
    {
        var typeName = typeof(T).Name;

        logger.LogInformation("Retrieving all {Type} entities", typeName);

        var response = await httpClient.GetAsync($"api/uper/all/{typeName}");
        await EnsureSuccessAsync(response, $"retrieving all {typeName}");

        var retVal = await response.Content.ReadFromJsonAsync<IEnumerable<T>>(JsonOptions);

        logger.LogInformation("Retrieved {Count} {Type} entities", retVal?.Count(), typeName);

        return retVal;
    }

    public async Task<T?> GetByIdAsync<T>(string id) where T : IEntity
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException($"{nameof(id)} cannot be empty.");
        }

        var typeName = typeof(T).Name;

        logger.LogInformation("Retrieving {Type} entity with id {Id}", typeName, id);

        var response = await httpClient.GetAsync($"api/uper/{typeName}/{id}");
        await EnsureSuccessAsync(response, $"retrieving {typeName} with id {id}");
        var retVal = await response.Content.ReadFromJsonAsync<T>(JsonOptions);

        logger.LogInformation("Retrieved {Type} entity with id {Id}", typeName, id);

        return retVal;
    }

    public async Task UpdateAsync<T>(T entity) where T : IEntity
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new ArgumentException("Entity Id cannot be empty.");
        }

        var typeName = typeof(T).Name;

        logger.LogInformation("Updating {Type} entity with id {Id}", typeName, entity.Id);

        var json = GetCreateUpdateJson(entity);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync("api/uper/update", content);

        await EnsureSuccessAsync(response, $"updating {typeName}");

        logger.LogInformation("{Type} entity with id {Id} updated", typeName, entity.Id);
    }

    private static string GetCreateUpdateJson<T>(params T[] entities) where T : IEntity
    {
        if (entities == null || entities.Length == 0)
        {
            throw new ArgumentException("At least one entity must be provided.", nameof(entities));
        }

        var data = new
        {
            Type = typeof(T).Name,
            Objects = entities
        };

        return JsonSerializer.Serialize(data, JsonOptions);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, string context)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            logger.LogError(
                "Error during {Context}. Status: {StatusCode}\nResponse: {ErrorDetails}",
                context,
                response.StatusCode,
                errorDetails
            );

            throw new HttpRequestException(
                $"Error during {context}. Status: {response.StatusCode}. Details: {errorDetails}"
            );
        }
    }
}
