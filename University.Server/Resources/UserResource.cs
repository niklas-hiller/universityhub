using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class UserResource
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public EAuthorization Authorization { get; set; }

        public List<Assignment>? Assignments { get; set; }
    }
}
