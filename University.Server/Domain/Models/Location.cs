namespace University.Server.Domain.Models
{
    public class Location : Base
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Size { get; set; }
    }
}
