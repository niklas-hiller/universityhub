namespace University.Server.Domain.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        // public Dictionary<string, string> Assets { get; set; } = new Dictionary<string, string>();
    }
}
