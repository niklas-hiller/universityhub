using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class UpdateLocationResource
    {
        [Required]
        [MinLength(5), MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public int Size { get; set; }
    }
}
