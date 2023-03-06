using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class SemesterEntity : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<SemesterModuleEntity> SemesterModules { get; set; } = new List<SemesterModuleEntity>();
    }
}
