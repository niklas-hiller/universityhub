namespace University.Server.Domain.Models
{
    public class Module : Base
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CreditPoints { get; set; }
        public EModuleType ModuleType { get; set; }
        public string MaxSize { get; set; }

        public ICollection<User> Professors { get; set; } = new List<User>();
    }
}
