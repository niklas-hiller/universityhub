using System.ComponentModel.DataAnnotations;
using University.Server.Domain.Models;

namespace University.Server.Resources
{
    public class SaveLocationResource
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Coordinates Coordinates { get; set; }
        // public Dictionary<string, string> Assets { get; set; }
    }
}
