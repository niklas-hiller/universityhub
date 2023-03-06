namespace University.Server.Domain.Models
{
    public class Semester : Base
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<SemesterModule> Modules { get; set; } = new List<SemesterModule>();
    }
}
