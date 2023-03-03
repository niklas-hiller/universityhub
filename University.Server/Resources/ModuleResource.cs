using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class ModuleResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreditPoints { get; set; }
    }
}
