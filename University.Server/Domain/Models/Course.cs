namespace University.Server.Domain.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Students { get; set; } = new List<User>();
        public List<Module> CompulsoryModules { get; set; } = new List<Module>();
        public List<Module> OptionalModules { get; set; } = new List<Module>();
    }
}
