using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class ModuleResource
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CreditPoints { get; set; }
        public List<Professor> AvailableProfessors { get; set; }
    }
}
