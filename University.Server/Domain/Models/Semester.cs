namespace University.Server.Domain.Models
{
    public class Semester
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SemesterModule> Modules { get; set; }
    }
}
