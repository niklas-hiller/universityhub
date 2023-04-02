using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class LocationEntity : BaseEntity
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Size { get; set; }
    }
}