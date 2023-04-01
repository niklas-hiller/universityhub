using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources.Request
{
    public class SaveModuleResource
    {
        [Required]
        [MinLength(5), MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        [Required]
        public int CreditPoints { get; set; }
        [Required]
        public EModuleType ModuleType { get; set; }
        [Required]
        public int MaxSize { get; set; }
    }
}
