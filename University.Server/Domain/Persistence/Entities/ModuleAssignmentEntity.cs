using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class ModuleAssignmentEntity : BaseEntity
    {
        public EModuleStatus Status { get; set; }

        public Guid ReferenceModuleId { get; set; }
    }
}
