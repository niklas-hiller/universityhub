using University.Server.Domain.Models;

namespace University.Server.Resources.Response
{
    public class LocationResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Size { get; set; }
    }
}