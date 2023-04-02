using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace University.Server.Domain.Persistence.Entities
{
    public class BaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
        }
    }
}