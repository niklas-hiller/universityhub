namespace University.Server.Domain.Models
{
    public class University
    {
        public Guid Id { get; set; }
        public List<Module> Modules { get; set; }
        public List<Semester> Semesters { get; set; }
        public List<User> Users { get; set; }
        public List<Course> Courses { get; set; }
    }
}
