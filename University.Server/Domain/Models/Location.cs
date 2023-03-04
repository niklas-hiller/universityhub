namespace University.Server.Domain.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Seats { get; set; }
    }
}
