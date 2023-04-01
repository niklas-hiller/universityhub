using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class SaveSemesterResource
    {
        [Required]
        [MinLength(5), MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
