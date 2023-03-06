using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace University.Server.Domain.Models
{
    public class Base
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
        }
    }
}
