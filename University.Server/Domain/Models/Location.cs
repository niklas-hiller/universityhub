namespace University.Server.Domain.Models
{
    public class Location : Archivable
    {
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
        public int Size { get; set; }
    }
}