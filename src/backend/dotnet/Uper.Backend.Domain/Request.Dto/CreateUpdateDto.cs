using System.Text.Json.Serialization;

namespace Uper.Backend.Domain.Request.Dto
{
    public class CreateUpdateDto
    {
        /// <summary>
        /// The name of the type (table) the objects belong to.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// An array of objects to be created or updated.
        /// </summary>
        [JsonPropertyName("objects")]
        public required List<Dictionary<string, object?>> Objects { get; set; }
    }
}