using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class UpdateCourseResource
    {
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
