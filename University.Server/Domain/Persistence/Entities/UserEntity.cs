using University.Server.Domain.Models;

namespace University.Server.Domain.Persistence.Entities
{
    public class UserEntity : ArchivableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public EAuthorization Authorization { get; set; }

        public ICollection<AssignmentEntity> Assignments { get; set; } = new List<AssignmentEntity>();
    }
}