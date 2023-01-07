namespace University.Server.Domain.Models
{
    public class Professor : User
    {
        public List<Module> PreferencedModules { get; set; }
    }
}
