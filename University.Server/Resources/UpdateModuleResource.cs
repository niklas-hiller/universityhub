using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class UpdateModuleResource
    {
        [MaxLength(30)]
        public string? Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public int CreditPoints { get; set; }
    }
}
