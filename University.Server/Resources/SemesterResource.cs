using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class SemesterResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<SemesterModule> Modules { get; set; }
    }
}
