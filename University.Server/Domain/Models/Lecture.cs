namespace University.Server.Domain.Models
{
    public class Lecture : Base
    {
        public int Duration { get; set; }
        public Location Location { get; set; }
        public DateTime Date { get; set; }
    }
}
