using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class SaveModuleResource
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 -]{5,30}$")]
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
