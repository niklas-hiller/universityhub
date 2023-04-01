using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources.Request
{
    public class SaveCourseResource
    {
        [Required]
        [MinLength(5), MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
