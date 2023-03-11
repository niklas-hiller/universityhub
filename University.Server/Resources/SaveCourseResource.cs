using System.ComponentModel.DataAnnotations;

namespace University.Server.Resources
{
    public class SaveCourseResource
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
