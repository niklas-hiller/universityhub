using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class UserResource
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EAuthorization Authorization { get; set; }
    }
}
