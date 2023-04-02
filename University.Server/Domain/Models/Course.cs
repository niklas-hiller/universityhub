namespace University.Server.Domain.Models
{
    public class Course : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<User> Students { get; set; } = new List<User>();
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}