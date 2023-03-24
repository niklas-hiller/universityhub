namespace University.Server.Domain.Models
{
    public class PatchModules
    {
        public ICollection<Module> Add { get; set; } = new List<Module>();
        public ICollection<Module> Remove { get; set; } = new List<Module>();
    }
}
