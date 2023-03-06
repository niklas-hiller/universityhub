using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EAuthorization Authorization { get; set; }

        public ICollection<ModuleAssignmentEntity> ModuleAssignments { get; set; }
    }
}
