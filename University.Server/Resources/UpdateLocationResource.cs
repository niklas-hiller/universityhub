using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class UpdateLocationResource
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 -]{5,30}$")]
        public string Name { get; set; }
        [Required]
        public int Size { get; set; }
    }
}
