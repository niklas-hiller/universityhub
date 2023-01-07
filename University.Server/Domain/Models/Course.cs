namespace University.Server.Domain.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public List<Module> CompulsoryModules { get; set; }
        public List<Module> OptionalModules { get; set; }
    }
}
