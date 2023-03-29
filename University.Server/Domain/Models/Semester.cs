namespace University.Server.Domain.Models
{
    public class Semester : Base
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; } = false;

        public ICollection<SemesterModule> Modules { get; set; } = new List<SemesterModule>();
    }
}
