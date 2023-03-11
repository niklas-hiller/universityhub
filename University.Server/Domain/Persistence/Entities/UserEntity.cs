using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public EAuthorization Authorization { get; set; }

        public ICollection<AssignmentEntity> ModuleAssignments { get; set; }
    }
}
