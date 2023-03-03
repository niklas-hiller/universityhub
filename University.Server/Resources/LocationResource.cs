using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class LocationResource
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Seats { get; set; }
    }
}
