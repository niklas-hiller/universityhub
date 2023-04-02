using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources.Request
{
    public class SaveLocationResource
    {
        [Required]
        [MinLength(5), MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public Coordinates Coordinates { get; set; }

        [Required]
        public int Size { get; set; }
    }
}