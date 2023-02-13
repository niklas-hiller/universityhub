using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class LocationResource
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        // public Dictionary<string, string> Assets { get; set; }
    }
}
