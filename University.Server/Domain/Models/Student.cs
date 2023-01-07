namespace University.Server.Domain.Models
{
    public class Student : User
    {
        public List<ModuleAssignment> ModuleAssignments { get; set; }
        public Course Course { get; set; }
    }
}
