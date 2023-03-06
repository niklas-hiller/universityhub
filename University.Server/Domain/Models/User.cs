namespace University.Server.Domain.Models
{
    public class User : Base
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EAuthorization Authorization { get; set; }

        public ICollection<ModuleAssignment> ModuleAssignments { get; set; }
    }
}
