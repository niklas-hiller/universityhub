using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class SaveSemesterResource
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
