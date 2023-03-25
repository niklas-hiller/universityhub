using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class SaveLocationResource
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 -]{5,30}$")]
        public string Name { get; set; }
        [Required]
        public Coordinates Coordinates { get; set; }
        [Required]
        public int Size { get; set; }
    }
}
