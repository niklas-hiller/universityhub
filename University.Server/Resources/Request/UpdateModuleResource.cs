using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class UpdateModuleResource
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 -]{5,30}$")]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        [Required]
        public int CreditPoints { get; set; }
        [Required]
        public int MaxSize { get; set; }
    }
}
